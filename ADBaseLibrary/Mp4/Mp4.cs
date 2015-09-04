using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary.Mp4
{
    public class Mp4
    {
        public static void Mp4Hash(string inpath, string outpath)
        {


            FileInfo f = new FileInfo(inpath);
            Stream i = File.OpenRead(inpath);
            BinaryReader reader = new BinaryReader(i);
            Stream o = File.Open(outpath,FileMode.Create,FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(o);
            Atom atm =new Atom();
            atm.Size = f.Length+8;
            atm.Read(reader);
            atm.RemoveAll(d => d.Name == "free");
            Atom mdat = atm.Find(d => d.Name == "mdat");
            long original = mdat.Position;
            atm.Remove(mdat);
            atm.Add(mdat);
            Atom fnd= atm.Find(d => d.Name == "moov");
            fnd.RemoveAll(d => d.Name == "udta");
            fnd.ReCalcSize();
            atm.ReCalcPosition();
            //Relocate 
            foreach (Atom a in fnd)
            {
                a.ReadData(reader);
                byte[] nm=new byte[4];
                byte[] nm2 = new byte[8];
                for (int x = 0; x < a.Size - 16;x++)
                {
                    Array.Copy(a.Data,x,nm,0,4);
                    string name = Encoding.ASCII.GetString(nm);
                    if ((name == "stco") && (x-4>=0))
                    {
                        Array.Copy(a.Data,x-4,nm,0,4);
                        Array.Reverse(nm);
                        uint size = BitConverter.ToUInt32(nm, 0);
                        if (size + x + 4 > a.Size)
                            continue;
                        Array.Copy(a.Data,x+8,nm, 0,4);
                        Array.Reverse(nm);
                        uint cnt = BitConverter.ToUInt32(nm, 0);
                        if (x+12+cnt*4>a.Size)
                            continue;
                        for (int j = 0; j < cnt; j++)
                        {
                            Array.Copy(a.Data,x+12+j*4,nm,0,4);
                            Array.Reverse(nm);
                            int offset = BitConverter.ToInt32(nm, 0);
                            offset += (int)(mdat.ExpectedPosition-original);
                            byte[] ne = BitConverter.GetBytes(offset);
                            Array.Reverse(ne);
                            Array.Copy(ne,0,a.Data,x+12+j*4,4);
                        }
                    }
                    else if ((name == "co64") && (x - 4 >= 0))
                    {
                        Array.Copy(a.Data, x - 4, nm, 0, 4);
                        Array.Reverse(nm);
                        uint size = BitConverter.ToUInt32(nm, 0);
                        if (size + x + 4 > a.Size)
                            continue;
                        Array.Copy(a.Data, x + 8, nm, 0, 4);
                        Array.Reverse(nm);
                        uint cnt = BitConverter.ToUInt32(nm, 0);
                        if (x + 12 + cnt * 8 > a.Size)
                            continue;
                        for (int j = 0; j < cnt; j++)
                        {
                            Array.Copy(a.Data, x + 12 + j * 8, nm2, 0, 8);
                            Array.Reverse(nm2);
                            long offset = BitConverter.ToInt64(nm2, 0);
                            offset += (long)(mdat.ExpectedPosition - original);
                            byte[] ne = BitConverter.GetBytes(offset);
                            Array.Reverse(ne);
                            Array.Copy(ne, 0, a.Data, x + 12 + j * 8, 8);
                        }

                    }
                }
            }


            foreach (Atom a in atm)
                a.Write(writer);
            writer.Close();
            reader.Close();
            o.Close();
            i.Close();
   
        }


    }
}
