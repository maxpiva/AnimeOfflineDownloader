using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADBaseLibrary.AdobeHDS.FlashWrapper;
using ADBaseLibrary.Helpers;


//Portion of this implementation is based on the ADOBEHDS KSV Script

namespace ADBaseLibrary.AdobeHDS
{
    public class FragmentProcessor
    {
        public string KeyUrl { get; set; }
        public byte[] KeyData { get; set; }

        private Dictionary<int,List<Tag>> FragmentQueue=new Dictionary<int, List<Tag>> (); 


        private CookieCollection cookies;
        private NameValueCollection headers;
         
        private string uagent;
        private int timeout;
        private string referer;
        private IWebProxy proxy;
        private BootStrap bootstrap;
        private int retries;
        private int threads;
        private string outputfile;
        private Media media;
        private string baseurl;
        private string guid;
        private Manifest manifest;
        private const string FormatFrag = "{0}{1}Seg{2}-Frag{3}{4}?{5}";
        private const string FormatKey = "{0}?guid={1}&{2}";
        private string keydata = string.Empty;


        public const int INVALID = -1;
        public const int FRAMEFIX_STEP = 40;
        public const int AUDIO_PACKET = 0x8;
        public const int VIDEO_PACKET = 0x9;
        public const int SCRIPT_DATA_PACKET = 0x12;
        public const int CODEC_ID_AVC = 0x7;
        public const int CODEC_ID_AAC = 0xA;
        public const int AVC_SEQUENCE_HEADER = 0;
        public const int AAC_SEQUENCE_HEADER = 0;
        public const int AVC_NALU = 1;
        public const int AVC_SEQUENCE_END = 2;
        
        private int FixedWindow = 1000;
        private int BaseTimestamp = INVALID;
        private int NegativeTimestamp = INVALID;
        private int PreviousAudioTimestamp = INVALID;
        private int PreviousVideoTimestamp = INVALID;
        private bool aacHeaderWritten = false;
        private bool prevAacHeader = false;
        private bool avcHeaderWritten = false;
        private bool prevAvcHeader = false;
        private bool hasAudio = false;
        private bool hasVideo = false;
        private Stream writeStream;


        private byte[] FlvHeader = { 0x46,0x4c,0x56,0x01,0x05,0x00,0x00,0x00,0x09,0x00,0x00,0x00,0x00 };

        public FragmentProcessor(CookieCollection cookies, NameValueCollection headers, string uagent, int timeout, string referer, IWebProxy proxy, int threads, int retries, string outputfile)
        {
            this.cookies = cookies;
            this.uagent = uagent;
            this.headers = headers;
            this.timeout = timeout;
            this.referer = referer;
            this.proxy = proxy;
            this.retries = retries;
            this.threads = threads;
            this.outputfile = outputfile;
        }

        public async Task Start(string baseurl, string guid, Manifest manifest, Media media, CancellationToken token,IProgress<double> progress)
        {
            this.media = media;
            this.guid = guid;
            this.baseurl = baseurl;
            this.bootstrap = media.Info.Info;
            this.manifest = manifest;
            await InitFile();
            await DoNextFragment();
            progress.Report((double)bootstrap.CurrentFragmentWrite * 100D/(double)bootstrap.FragmentCount);
            token.ThrowIfCancellationRequested();
            await ThreadProcessor(token, progress);
            token.ThrowIfCancellationRequested();
        }

        public async Task InitFile()
        {
            writeStream = Stream.Synchronized(File.Open(outputfile, FileMode.Create, FileAccess.Write));
            await writeStream.WriteAsync(FlvHeader, 0, FlvHeader.Length);
            if (media.Metadata != null)
            {
                byte[] metadata = Convert.FromBase64String(media.Metadata);
                byte[] header = new byte[11];
                header[0] = SCRIPT_DATA_PACKET;
                header[1] = (byte)(metadata.Length >> 16);
                header[2] = (byte) ((metadata.Length >> 8) & 0xFF);
                header[3] = (byte) (metadata.Length & 0xff);
                metadata[metadata.Length - 1] = 0x9;
                await writeStream.WriteAsync(header, 0, 11);
                await writeStream.WriteAsync(metadata, 0, metadata.Length);
                byte[] finalsize=BitConverter.GetBytes((int) (metadata.Length + 11));
                Array.Reverse(finalsize);
                await writeStream.WriteAsync(finalsize, 0, 4);
            }
        }

        public async Task CloseFile()
        {
            byte tp = (byte)(hasVideo ? 4 : 0);
            tp |= (byte)(hasAudio ? 1 : 0);
            byte[] data=new byte[1];
            data[0] = tp;
            writeStream.Seek(4, SeekOrigin.Begin);
            await writeStream.WriteAsync(data, 0, 1);
            writeStream.Close();
        } 
        private int ThreadCount = 0;

        private async Task ThreadProcessor(CancellationToken token, IProgress<double> progress)
        {
            List<Task> FragmentTasks = new List<Task>();
            do
            {
                while (ThreadCount < threads && bootstrap.CurrentFragmentWrite != bootstrap.FragmentCount && !token.IsCancellationRequested)
                {
                    FragmentTasks.Add(DoNextFragment());
                    ThreadCount++;
                }
                Task f=await Task.WhenAny(FragmentTasks);
                await f;
                FragmentTasks.Remove(f);
                progress.Report((double)bootstrap.CurrentFragmentWrite * 100D / (double)bootstrap.FragmentCount);
                ThreadCount--;
            } while (bootstrap.CurrentFragmentWrite != bootstrap.FragmentCount && !token.IsCancellationRequested);
            await CloseFile();
        }
        public async Task DoNextFragment()
        {
            KeyValuePair<int, int>? v = bootstrap.GetNextRead();
            if (v == null)
                return;
            string url = string.Format(FormatFrag, baseurl, media.Url, v.Value.Key, v.Value.Value+1, keydata, manifest.Pv20.Substring(1));
            WebStream ws = await GetStream(url);
            if (ws == null || ws.StatusCode != HttpStatusCode.OK)
            {
                ws?.Dispose();
                throw new IOException("Error " + ws.StatusCode + " trying to download fragment");
            }
            await ProcessFragment(ws, v.Value.Value);
            ws?.Dispose();
        }


        private async Task ProcessFragment(Stream s, int fragment)
        {
            BoxReader reader=new BoxReader(s);
            string name;
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                long size = reader.ReadHeader(out name);
                if (name == "abst")
                    reader.ReadBootStrap(bootstrap);
                else if (name == "mdat")
                {
                    long max = reader.BaseStream.Position + size;
                    List<Tag> tags=new List<Tag>();
                    while (reader.BaseStream.Position != max)
                    {
                        tags.Add(await GetNextTag(reader.BaseStream));
                    }
                    await QueueTags(tags, fragment);
                }
                else
                    reader.BaseStream.Seek(size, SeekOrigin.Current);
            }
            reader.Dispose();
        }
        private SemaphoreSlim queueLock=new SemaphoreSlim(1);


        private void CallAkamaiDecryptionService(List<Tag> ts)
        {
            

            StringBuilder datas = new StringBuilder();
            StringBuilder keys = new StringBuilder();
            int cnt = 0;
            foreach (Tag t in ts)
            {
                if (cnt > 0)
                {
                    datas.Append(",");
                    keys.Append(",");
                }

                if (t.NeedDecryption)
                {
                    datas.Append(Convert.ToBase64String(t.Packet));
                    keys.Append(Convert.ToBase64String(t.Key));
                    cnt++;
                }
            }
            if (datas.Length > 0)
            {
                string result = DecryptForm.Decrypt(datas.ToString(), keys.ToString());
                string[] splits = result.Split(',');
                cnt = 0;
                foreach (Tag t in ts)
                {
                    if (t.NeedDecryption)
                    {
                        t.NeedDecryption = false;
                        t.Packet = Convert.FromBase64String(splits[cnt]);
                        t.PacketSize = t.Packet.Length;
                        t.TotalSize = t.PacketSize + 11;
                        cnt++;
                    }
                }
            }
        }

        private async Task QueueTags(List<Tag> ts,int fragment)
        {
            CallAkamaiDecryptionService(ts);
            queueLock.Wait();
            try
            {
                if (fragment == bootstrap.CurrentFragmentWrite)
                {
                    await ProcessTags(ts);
                    bootstrap.IncreaseFragmentWrite();
                    while (FragmentQueue.ContainsKey(bootstrap.CurrentFragmentWrite))
                    {
                        await ProcessTags(FragmentQueue[bootstrap.CurrentFragmentWrite]);
                        FragmentQueue.Remove(bootstrap.CurrentFragmentWrite);
                        bootstrap.IncreaseFragmentWrite();
                    }
                }
                else
                {
                    FragmentQueue.Add(fragment, ts);
                }
            }
            catch (Exception e)
            {
                int b = 1;
            }
            finally
            {
                queueLock.Release();
            }

        }

        private async Task WriteTag(Tag tag)
        {
            byte[] header=new byte[11];
            header[0] = tag.Type;
            header[1] = (byte)(tag.PacketSize >> 16);
            header[2] = (byte)((tag.PacketSize >> 8) & 0xFF);
            header[3] = (byte) (tag.PacketSize & 0xFF);
            header[4] = (byte)((tag.TimeStamp >> 16) & 0xFF);
            header[5] = (byte)((tag.TimeStamp >> 8) & 0xFF);
            header[6] = (byte)(tag.TimeStamp & 0xFF);
            header[7] = (byte)(tag.TimeStamp >>24);
            Array.Copy(tag.Remainder, 0, header, 8, 3);
            byte[] tsize = BitConverter.GetBytes(tag.TotalSize);
            Array.Reverse(tsize);
            await writeStream.WriteAsync(header, 0, 11);
            await writeStream.WriteAsync(tag.Packet, 0, tag.PacketSize);
            await writeStream.WriteAsync(tsize, 0, 4);
        }
        private async Task ProcessTag(Tag tag)
        {
            
            int CurrentTimestamp=tag.TimeStamp;
            int LastTimestamp=PreviousVideoTimestamp >=PreviousAudioTimestamp ? PreviousVideoTimestamp : PreviousAudioTimestamp;
            int FixedTimestamp = LastTimestamp + FRAMEFIX_STEP;
            if (BaseTimestamp==INVALID && (tag.Type==AUDIO_PACKET || tag.Type==VIDEO_PACKET))
                BaseTimestamp=tag.TimeStamp;
            if (BaseTimestamp > FixedWindow && tag.TimeStamp >= BaseTimestamp)
                tag.TimeStamp = BaseTimestamp;
            if (LastTimestamp != INVALID)
            {
                int timeShift = tag.TimeStamp - LastTimestamp;
                if (timeShift > FixedWindow)
                {
                    if (BaseTimestamp < tag.TimeStamp)
                        BaseTimestamp += timeShift - FRAMEFIX_STEP;
                    else
                        BaseTimestamp -= timeShift - FRAMEFIX_STEP;
                    tag.TimeStamp = FixedTimestamp;
                }
                else
                {
                    LastTimestamp = tag.Type == VIDEO_PACKET ? PreviousVideoTimestamp : PreviousAudioTimestamp;
                    if (tag.TimeStamp < (LastTimestamp - FixedWindow))
                    {
                        if ((NegativeTimestamp != INVALID) &&
                            ((tag.TimeStamp + NegativeTimestamp) < (LastTimestamp - FixedWindow)))
                            NegativeTimestamp = INVALID;
                        if (NegativeTimestamp == INVALID)
                        {
                            NegativeTimestamp = FixedTimestamp - tag.TimeStamp;
                            tag.TimeStamp = FixedTimestamp;
                        }
                        else
                        {
                            if ((tag.TimeStamp + NegativeTimestamp) <= (LastTimestamp + FixedWindow))
                                tag.TimeStamp += NegativeTimestamp;
                            else
                            {
                                NegativeTimestamp = FixedTimestamp - tag.TimeStamp;
                                tag.TimeStamp = FixedTimestamp;
                            }
                        }
                    }
                }
            }
            switch (tag.Type)
            {
                case AUDIO_PACKET:
                    if (tag.TimeStamp > PreviousAudioTimestamp - FixedWindow)
                    {
                        int frameinfo = tag.Packet[0];
                        int codec = (frameinfo & 0xF0) >> 4;
                        byte ptype = tag.Packet[1];
                        if (codec == CODEC_ID_AAC)
                        {
                            if (ptype == AAC_SEQUENCE_HEADER)
                            {
                                if (aacHeaderWritten)
                                    break;
                                aacHeaderWritten = true;
                            }
                            else if (!aacHeaderWritten)
                                break;
                        }
                        if (tag.PacketSize > 0)
                        {
                            if (!((codec == CODEC_ID_AAC) && ((ptype == AAC_SEQUENCE_HEADER) || prevAacHeader)))
                            {
                                if ((PreviousAudioTimestamp != INVALID) && (tag.TimeStamp <= PreviousAudioTimestamp))
                                    tag.TimeStamp += (FRAMEFIX_STEP/5) + (PreviousAudioTimestamp - tag.TimeStamp);
                            }
                            await WriteTag(tag);
                            prevAacHeader = ((codec == CODEC_ID_AAC) && (ptype == AAC_SEQUENCE_HEADER));
                            PreviousAudioTimestamp = tag.TimeStamp;
                        }
                    }
                    hasAudio = true;
                    break;
                case VIDEO_PACKET:
                    if (tag.TimeStamp > PreviousVideoTimestamp - FixedWindow)
                    {
                        int frameinfo = tag.Packet[0];
                        int ftype = (frameinfo & 0xF0) >> 4;
                        int codec = frameinfo & 0xF;
                        byte ptype = tag.Packet[1];
                        if (codec == CODEC_ID_AVC)
                        {
                            if (ptype == AVC_SEQUENCE_HEADER)
                            {
                                if (avcHeaderWritten)
                                    break;
                                avcHeaderWritten = true;
                            }
                            else if (!avcHeaderWritten)
                                break;
                        }
                        if (tag.PacketSize > 0)
                        {
                            if (!((codec == CODEC_ID_AVC) && ((ptype == AVC_SEQUENCE_HEADER) || (ptype == AVC_SEQUENCE_END) || prevAvcHeader)))
                            {
                                if ((PreviousVideoTimestamp != INVALID) && (tag.TimeStamp <= PreviousVideoTimestamp))
                                {
                                    tag.TimeStamp += (FRAMEFIX_STEP/5) | (PreviousVideoTimestamp - tag.TimeStamp);
                                }
                            }
                            await WriteTag(tag);
                            prevAvcHeader = ((codec == CODEC_ID_AVC) && (ptype == AVC_SEQUENCE_HEADER));
                            PreviousVideoTimestamp = tag.TimeStamp;
                        }
                    }
                    hasVideo = true;
                    break;
          
            }
        }
        private async Task ProcessTags(List<Tag> tags)
        {
            foreach (Tag t in tags)
                await ProcessTag(t);
        }
        private async Task<WebStream> GetStream(string url)
        {
            int cnt = 0;
            do
            {
                WebStream ws = await WebStream.Get(url,null, uagent, headers, cookies, timeout, true, referer, proxy);
                if (ws == null || ws.StatusCode != HttpStatusCode.OK)
                {
                    ws?.Dispose();
                    cnt++;
                    if (cnt == retries)
                        return ws;
                }
                else
                    return ws;
            } while (true);

        }

        public class Tag
        {
            public byte Type;
            public int PacketSize;
            public int TimeStamp;
            public byte[] Remainder;
            public byte[] Packet;
            public int TotalSize;
            public bool NeedDecryption;
            public byte[] Key;
        }



        public async Task<Tag> GetNextTag(Stream stream)
        {
            Tag t = new Tag();
            byte[] header = new byte[11];
            byte[] remainder=new byte[4];
            stream.Read(header, 0, 11);
            t.Type = header[0];
            t.PacketSize = header[1] << 16 | header[2] << 8 | header[3] & 255;
            t.TimeStamp = (header[7]<<24 | header[4] << 16 | header[5] << 8 | header[6] & 255)& 0x7FFFFFFF;
            t.Remainder=new byte[3];
            Array.Copy(header,8,t.Remainder,0,3);
            byte[] data = new byte[t.PacketSize];
            stream.Read(data, 0, t.PacketSize);
            stream.Read(remainder, 0, 4);
            Array.Reverse(remainder);
            t.TotalSize = BitConverter.ToInt32(remainder, 0);
            t.NeedDecryption = false;
            
            if (t.Type == 10 || t.Type == 11)
            {
                t.Type -= 2;
                byte tp = data[12];
                int pos = 13;
                if ((tp & 2) > 0)
                    pos += 16;
                if ((tp & 4) > 0)
                {
                    StringBuilder bld = new StringBuilder();
                    byte b = data[pos++];
                    while (b != 0)
                    {
                        bld.Append((char) b);
                        b = data[pos++];
                    }
                    string nurl = bld.ToString();
                    if (nurl != KeyUrl)
                    {
                        Uri bas=new Uri(baseurl);

                        string url= string.Format(FormatKey,bas.Scheme + "://" + bas.Host+nurl,guid,manifest.Pv20.Substring(1)+ "&hdcore=3.2.0");
                        int idx = nurl.IndexOf("_");
                        keydata = nurl.Substring(idx);
                        WebStream ws = await GetStream(url);
                        if (ws == null || ws.StatusCode != HttpStatusCode.OK)
                        {
                            ws?.Dispose();
                            throw new IOException("Error " + ws.StatusCode + " trying to download fragment");
                        }
                        KeyData = new byte[ws.ContentLength];
                        await ws.ReadAsync(KeyData, 0, (int)ws.ContentLength);
                        ws.Dispose();
                        KeyUrl = nurl;
                    }
                }
                t.NeedDecryption = true;
                t.Key = KeyData;
            }
            t.Packet = data;
            t.PacketSize = data.Length;
            return t;
        }

    }
}
