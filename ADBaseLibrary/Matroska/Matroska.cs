using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using ADBaseLibrary.Matroska.Objects;

namespace ADBaseLibrary.Matroska
{
    public static class Matroska
    {

        internal static void FixUIDS(Container c)
        {
            //Change UIDS to numeric ids.
            //Fix Track IDS
            List<EbmlGeneric> tracks = c.FindAll(Ids.MATROSKA_ID_TRACKENTRY);
            List<EbmlGeneric> gg = c.FindAll(Ids.MATROSKA_ID_TAGTARGETS_TRACKUID);
            List<EbmlGeneric> planes = c.FindAll(Ids.MATROSKA_ID_TRACKPLANEUID);
            foreach (EbmlMaster j in tracks)
            {
                EbmlUint u = (EbmlUint)j.FindFirst(Ids.MATROSKA_ID_TRACKUID);
                EbmlUint tn = (EbmlUint)j.FindFirst(Ids.MATROSKA_ID_TRACKNUMBER);
                foreach (EbmlUint n in planes)
                {
                    if (n.Value == u.Value)
                    {
                        n.Value = tn.Value;
                        break;
                    }
                }
                foreach (EbmlUint n in gg)
                {
                    ulong nval = 0;
                    if (n.Value == u.Value)
                    {
                        n.Value = tn.Value;
                        break;
                    }
                }
                u.Value = tn.Value;
            }
            //Fix Chapters
            List<EbmlGeneric> chapters = c.FindAll(Ids.MATROSKA_ID_EDITIONENTRY);
            gg = c.FindAll(Ids.MATROSKA_ID_TAGTARGETS_CHAPTERUID);
            ulong cnt = 1;
            foreach (EbmlMaster j in chapters)
            {
                EbmlUint u = (EbmlUint)j.FindFirst(Ids.MATROSKA_ID_CHAPTERUID);
                foreach (EbmlUint n in gg)
                {
                    if (n.Value == u.Value)
                    {
                        n.Value = cnt;
                        break;
                    }
                }
                u.Value = cnt;
                u = (EbmlUint)j.FindFirst(Ids.MATROSKA_ID_EDITIONUID);
                if (u != null)
                    u.Value = cnt;
                cnt++;
            }
            //Fix Attachments
            List<EbmlGeneric> attachments = c.FindAll(Ids.MATROSKA_ID_ATTACHEDFILE);
            gg = c.FindAll(Ids.MATROSKA_ID_TAGTARGETS_ATTACHUID);
            cnt = 1;
            foreach (EbmlMaster j in chapters)
            {
                EbmlUint u = (EbmlUint)j.FindFirst(Ids.MATROSKA_ID_FILEUID);
                foreach (EbmlUint n in gg)
                {
                    if (n.Value == u.Value)
                    {
                        n.Value = cnt;
                        break;
                    }
                }
                u.Value = cnt;
                cnt++;
            }
        }

        internal static EbmlBinary NormalizeHeader(Container c)
        {
            //Remove DATEUTC
            EbmlMaster info = (EbmlMaster)c.FindFirst(Ids.MATROSKA_ID_INFO);
            EbmlBinary date = (EbmlBinary)info.FindFirst(Ids.MATROSKA_ID_DATEUTC);
            if (date != null)
                info.Value.Remove(date);
            //Changing Muxing and Writting App to a Constant.
            EbmlUtf8 muxapp = (EbmlUtf8)info.FindFirst(Ids.MATROSKA_ID_MUXINGAPP);
            if (muxapp == null)
            {
                muxapp = new EbmlUtf8 { Id = Ids.MATROSKA_ID_MUXINGAPP };
                info.Value.Add(muxapp);
            }
            EbmlUtf8 wrtapp = (EbmlUtf8)info.FindFirst(Ids.MATROSKA_ID_WRITINGAPP);
            if (wrtapp == null)
            {
                wrtapp = new EbmlUtf8 { Id = Ids.MATROSKA_ID_WRITINGAPP };
                info.Value.Add(wrtapp);
            }
            muxapp.Value = wrtapp.Value = "AOD";
            EbmlBinary seguid = (EbmlBinary)info.FindFirst(Ids.MATROSKA_ID_SEGMENTUID);
            if (seguid == null)
            {
                seguid = new EbmlBinary { Id = Ids.MATROSKA_ID_SEGMENTUID, Value = new byte[16] };
                info.Value.Add(seguid);
            }

            info.Value.Sort();
            return seguid;
        }

        internal static void MoveTagInfoToTracks(Container c)
        {
            //Moving Title and Language tags to tracks if they exists
            List<EbmlGeneric> tracks = c.FindAll(Ids.MATROSKA_ID_TRACKENTRY);
            List<EbmlGeneric> tags = c.FindAll(Ids.MATROSKA_ID_TAG);
            foreach (EbmlMaster t in tags)
            {
                EbmlMaster tt = (EbmlMaster)t.FindFirst(Ids.MATROSKA_ID_TAGTARGETS);
                if (tt != null)
                {
                    EbmlMaster sp = (EbmlMaster)t.FindFirst(Ids.MATROSKA_ID_SIMPLETAG);
                    EbmlUint jk = (EbmlUint)tt.FindFirst(Ids.MATROSKA_ID_TAGTARGETS_TRACKUID);
                    if (jk != null)
                    {
                        EbmlUtf8 name = (EbmlUtf8)sp.FindFirst(Ids.MATROSKA_ID_TAGNAME);
                        EbmlUtf8 value = (EbmlUtf8)sp.FindFirst(Ids.MATROSKA_ID_TAGSTRING);
                        if (name != null && value != null)
                        {
                            string nm = name.Value.ToUpper(CultureInfo.InvariantCulture);
                            string vl = value.Value;
                            if (nm == "LANGUAGE" || nm == "TITLE" || nm == "NAME")
                            {
                                foreach (EbmlMaster j in tracks)
                                {
                                    EbmlUint u = (EbmlUint)j.FindFirst(Ids.MATROSKA_ID_TRACKUID);
                                    if (u.Value == jk.Value)
                                    {
                                        EbmlUtf8 change;
                                        if (nm == "TITLE" || nm == "NAME")
                                        {
                                            change = (EbmlUtf8)j.FindFirst(Ids.MATROSKA_ID_TRACKNAME);
                                            if (change == null)
                                            {
                                                change = new EbmlUtf8 { Id = Ids.MATROSKA_ID_TRACKNAME };
                                                j.Value.Add(change);
                                            }
                                        }
                                        else
                                        {
                                            change = (EbmlUtf8)j.FindFirst(Ids.MATROSKA_ID_TRACKLANGUAGE);
                                            if (change == null)
                                            {
                                                change = new EbmlUtf8 { Id = Ids.MATROSKA_ID_TRACKLANGUAGE };
                                                j.Value.Add(change);
                                            }
                                        }
                                        change.Value = vl;
                                        j.Value.Sort();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static void Shrink(Container c)
        {
            c.RemoveAll(Ids.MATROSKA_ID_TAGS);
            c.RemoveAll(Ids.EBML_ID_VOID);
            c.RemoveAll(Ids.EBML_ID_CRC32);
        }

        public static void FixPositions(Container c)
        {

            EbmlMaster segment = (EbmlMaster)c.FindFirst(Ids.MATROSKA_ID_SEGMENT);
            bool repeatpass = false;

            List<EbmlGeneric> cuepositions = c.FindAll(Ids.MATROSKA_ID_CUECLUSTERPOSITION);
            List<EbmlGeneric> clusters = c.FindAll(Ids.MATROSKA_ID_CLUSTER);
            int cnt = 1;
            foreach (EbmlUint j in cuepositions)
            {
                ulong val = j.Value + segment.InputValueOffset;
                EbmlMaster repo = (EbmlMaster)clusters.First(a => a.InputOffset == val);
                if (repo.LinkedId != 0)
                    j.LinkedId = repo.LinkedId;
                else
                {
                    repo.LinkedId = j.LinkedId = cnt;
                    cnt++;
                }
            }

            do
            {
                repeatpass = false;
                //Propagate Expected Offsets
                c.PropagateExpectedOffset();
                //Reposition Cues
                foreach (EbmlUint j in cuepositions)
                {
                    EbmlMaster repo = (EbmlMaster)clusters.First(a => a.LinkedId==j.LinkedId);
                    ulong nvalue = repo.ExpectedOffset - segment.ExpectedValueOffset;
                    if (nvalue != j.Value)
                    {
                        repeatpass = true;
                        j.Value = nvalue;
                    }
                }
                if (!repeatpass)
                {
                    //Propagate Expected Offsets
                    c.PropagateExpectedOffset();
                    //Fix Seek Header
                    EbmlMaster smas = (EbmlMaster)c.FindFirst(Ids.MATROSKA_ID_SEEKHEAD);
                    List<EbmlGeneric> seeks = c.FindAll(Ids.MATROSKA_ID_SEEKENTRY);
                    foreach (EbmlMaster sk in seeks)
                    {
                        EbmlUint id = (EbmlUint)sk.FindFirst(Ids.MATROSKA_ID_SEEKID);
                        if (id != null)
                        {
                            EbmlUint pos = (EbmlUint)sk.FindFirst(Ids.MATROSKA_ID_SEEKPOSITION);
                            EbmlGeneric gen = segment.FindFirst(id.Value);
                            if (gen == null)
                            {
                                repeatpass = true;
                                smas.Value.Remove(sk);
                                break;
                            }
                            ulong nval = gen.ExpectedOffset - segment.ExpectedValueOffset;
                            if (pos.Value != nval)
                            {
                                pos.Value = nval;
                                repeatpass = true;
                            }
                        }
                    }
                }
            } while (repeatpass);

        }

        public static void MatroskaHash(string inpath, string outpath)
        {
            FileInfo f = new FileInfo(inpath);
            Stream i = File.OpenRead(inpath);
            BinaryReader reader = new BinaryReader(i);
            Container c = new Container();
            c.Read(reader, (ulong)f.Length);
            FixUIDS(c);
            EbmlBinary segmentuid = NormalizeHeader(c);
            MoveTagInfoToTracks(c);
            Shrink(c);
            FixPositions(c);
            Stream o = File.Open(outpath, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(o);
            MD5 md5 = MD5.Create();
            c.Write(writer, md5);
            //Using Cluster Data MD5 as segment UID
            byte[] b = new byte[0];
            md5.TransformFinalBlock(b, 0, 0);
            byte[] hash = md5.Hash;
            segmentuid.Value = hash;
            writer.BaseStream.Seek((long)segmentuid.OutputValueOffset, SeekOrigin.Begin);
            writer.Write(hash);
            writer.Close();
            reader.Close();
            i.Close();
            o.Close();
        }
    }
}
