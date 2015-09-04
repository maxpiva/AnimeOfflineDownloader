namespace ADBaseLibrary.Matroska
{
    public class Ids
    {
        //FROM FFMPEG

        /* EBML version supported */
        public const int EBML_VERSION = 1;
        /* top-level master-IDs */
        public const int EBML_ID_HEADER = 0x1A45DFA3;

        /* IDs in the HEADER master */
        public const int EBML_ID_EBMLVERSION = 0x4286;
        public const int EBML_ID_EBMLREADVERSION = 0x42F7;
        public const int EBML_ID_EBMLMAXIDLENGTH = 0x42F2;
        public const int EBML_ID_EBMLMAXSIZELENGTH = 0x42F3;
        public const int EBML_ID_DOCTYPE = 0x4282;
        public const int EBML_ID_DOCTYPEVERSION = 0x4287;
        public const int EBML_ID_DOCTYPEREADVERSION = 0x4285;
        
        /* general EBML types */
        public const int EBML_ID_VOID = 0xEC;
        public const int EBML_ID_CRC32 = 0xBF;

        /*
         * Matroska element IDs, max. 32 bits
         */

        /* toplevel segment */
        public const int MATROSKA_ID_SEGMENT = 0x18538067;

        /* Matroska top-level master IDs */
        public const int MATROSKA_ID_INFO = 0x1549A966;
        public const int MATROSKA_ID_TRACKS = 0x1654AE6B;
        public const int MATROSKA_ID_CUES = 0x1C53BB6B;
        public const int MATROSKA_ID_TAGS = 0x1254C367;
        public const int MATROSKA_ID_SEEKHEAD = 0x114D9B74;
        public const int MATROSKA_ID_ATTACHMENTS = 0x1941A469;
        public const int MATROSKA_ID_CLUSTER = 0x1F43B675;
        public const int MATROSKA_ID_CHAPTERS = 0x1043A770;

        /* IDs in the info master */
        public const int MATROSKA_ID_TIMECODESCALE = 0x2AD7B1;
        public const int MATROSKA_ID_DURATION = 0x4489;
        public const int MATROSKA_ID_TITLE = 0x7BA9;
        public const int MATROSKA_ID_WRITINGAPP = 0x5741;
        public const int MATROSKA_ID_MUXINGAPP = 0x4D80;
        public const int MATROSKA_ID_DATEUTC = 0x4461;
        public const int MATROSKA_ID_SEGMENTUID = 0x73A4;

        /* ID in the tracks master */
        public const int MATROSKA_ID_TRACKENTRY = 0xAE;

        /* IDs in the trackentry master */
        public const int MATROSKA_ID_TRACKNUMBER = 0xD7;
        public const int MATROSKA_ID_TRACKUID = 0x73C5;
        public const int MATROSKA_ID_TRACKTYPE = 0x83;
        public const int MATROSKA_ID_TRACKVIDEO = 0xE0;
        public const int MATROSKA_ID_TRACKAUDIO = 0xE1;
        public const int MATROSKA_ID_TRACKOPERATION = 0xE2;
        public const int MATROSKA_ID_TRACKCOMBINEPLANES = 0xE3;
        public const int MATROSKA_ID_TRACKPLANE = 0xE4;
        public const int MATROSKA_ID_TRACKPLANEUID = 0xE5;
        public const int MATROSKA_ID_TRACKPLANETYPE = 0xE6;
        public const int MATROSKA_ID_CODECID = 0x86;
        public const int MATROSKA_ID_CODECPRIVATE = 0x63A2;
        public const int MATROSKA_ID_CODECNAME = 0x258688;
        public const int MATROSKA_ID_CODECINFOURL = 0x3B4040;
        public const int MATROSKA_ID_CODECDOWNLOADURL = 0x26B240;
        public const int MATROSKA_ID_CODECDECODEALL = 0xAA;
        public const int MATROSKA_ID_CODECDELAY = 0x56AA;
        public const int MATROSKA_ID_SEEKPREROLL = 0x56BB;
        public const int MATROSKA_ID_TRACKNAME = 0x536E;
        public const int MATROSKA_ID_TRACKLANGUAGE = 0x22B59C;
        public const int MATROSKA_ID_TRACKFLAGENABLED = 0xB9;
        public const int MATROSKA_ID_TRACKFLAGDEFAULT = 0x88;
        public const int MATROSKA_ID_TRACKFLAGFORCED = 0x55AA;
        public const int MATROSKA_ID_TRACKFLAGLACING = 0x9C;
        public const int MATROSKA_ID_TRACKMINCACHE = 0x6DE7;
        public const int MATROSKA_ID_TRACKMAXCACHE = 0x6DF8;
        public const int MATROSKA_ID_TRACKDEFAULTDURATION = 0x23E383;
        public const int MATROSKA_ID_TRACKCONTENTENCODINGS = 0x6D80;
        public const int MATROSKA_ID_TRACKCONTENTENCODING = 0x6240;
        public const int MATROSKA_ID_TRACKTIMECODESCALE = 0x23314F;
        public const int MATROSKA_ID_TRACKMAXBLKADDID = 0x55EE;

        /* IDs in the trackvideo master */
        public const int MATROSKA_ID_VIDEOFRAMERATE = 0x2383E3;
        public const int MATROSKA_ID_VIDEODISPLAYWIDTH = 0x54B0;
        public const int MATROSKA_ID_VIDEODISPLAYHEIGHT = 0x54BA;
        public const int MATROSKA_ID_VIDEOPIXELWIDTH = 0xB0;
        public const int MATROSKA_ID_VIDEOPIXELHEIGHT = 0xBA;
        public const int MATROSKA_ID_VIDEOPIXELCROPB = 0x54AA;
        public const int MATROSKA_ID_VIDEOPIXELCROPT = 0x54BB;
        public const int MATROSKA_ID_VIDEOPIXELCROPL = 0x54CC;
        public const int MATROSKA_ID_VIDEOPIXELCROPR = 0x54DD;
        public const int MATROSKA_ID_VIDEODISPLAYUNIT = 0x54B2;
        public const int MATROSKA_ID_VIDEOFLAGINTERLACED = 0x9A;
        public const int MATROSKA_ID_VIDEOSTEREOMODE = 0x53B8;
        public const int MATROSKA_ID_VIDEOALPHAMODE = 0x53C0;
        public const int MATROSKA_ID_VIDEOASPECTRATIO = 0x54B3;
        public const int MATROSKA_ID_VIDEOCOLORSPACE = 0x2EB524;

/* IDs in the trackaudio master */
        public const int MATROSKA_ID_AUDIOSAMPLINGFREQ = 0xB5;
        public const int MATROSKA_ID_AUDIOOUTSAMPLINGFREQ = 0x78B5;

        public const int MATROSKA_ID_AUDIOBITDEPTH = 0x6264;
        public const int MATROSKA_ID_AUDIOCHANNELS = 0x9F;

        /* IDs in the content encoding master */
        public const int MATROSKA_ID_ENCODINGORDER = 0x5031;
        public const int MATROSKA_ID_ENCODINGSCOPE = 0x5032;
        public const int MATROSKA_ID_ENCODINGTYPE = 0x5033;
        public const int MATROSKA_ID_ENCODINGCOMPRESSION = 0x5034;
        public const int MATROSKA_ID_ENCODINGCOMPALGO = 0x4254;
        public const int MATROSKA_ID_ENCODINGCOMPSETTINGS = 0x4255;

        public const int MATROSKA_ID_ENCODINGENCRYPTION = 0x5035;
        public const int MATROSKA_ID_ENCODINGENCAESSETTINGS = 0x47E7;
        public const int MATROSKA_ID_ENCODINGENCALGO = 0x47E1;
        public const int MATROSKA_ID_ENCODINGENCKEYID = 0x47E2;
        public const int MATROSKA_ID_ENCODINGSIGALGO = 0x47E5;
        public const int MATROSKA_ID_ENCODINGSIGHASHALGO = 0x47E6;
        public const int MATROSKA_ID_ENCODINGSIGKEYID = 0x47E4;
        public const int MATROSKA_ID_ENCODINGSIGNATURE = 0x47E3;

        /* ID in the cues master */
        public const int MATROSKA_ID_POINTENTRY = 0xBB;
    
        /* IDs in the pointentry master */
        public const int MATROSKA_ID_CUETIME = 0xB3;
        public const int MATROSKA_ID_CUETRACKPOSITION = 0xB7;

        /* IDs in the cuetrackposition master */
        public const int MATROSKA_ID_CUETRACK = 0xF7;
        public const int MATROSKA_ID_CUECLUSTERPOSITION = 0xF1;
        public const int MATROSKA_ID_CUERELATIVEPOSITION = 0xF0;
        public const int MATROSKA_ID_CUEDURATION = 0xB2;
        public const int MATROSKA_ID_CUEBLOCKNUMBER = 0x5378;

        /* IDs in the tags master */
        public const int MATROSKA_ID_TAG = 0x7373;
        public const int MATROSKA_ID_SIMPLETAG = 0x67C8;
        public const int MATROSKA_ID_TAGNAME = 0x45A3;
        public const int MATROSKA_ID_TAGSTRING = 0x4487;
        public const int MATROSKA_ID_TAGLANG = 0x447A;
        public const int MATROSKA_ID_TAGDEFAULT = 0x4484;
        public const int MATROSKA_ID_TAGDEFAULT_BUG = 0x44B4;
        public const int MATROSKA_ID_TAGTARGETS = 0x63C0;
        public const int MATROSKA_ID_TAGTARGETS_TYPE = 0x63CA;
        public const int MATROSKA_ID_TAGTARGETS_TYPEVALUE = 0x68CA;
        public const int MATROSKA_ID_TAGTARGETS_TRACKUID = 0x63C5;
        public const int MATROSKA_ID_TAGTARGETS_CHAPTERUID = 0x63C4;
        public const int MATROSKA_ID_TAGTARGETS_ATTACHUID = 0x63C6;

        /* IDs in the seekhead master */
        public const int MATROSKA_ID_SEEKENTRY = 0x4DBB;

        /* IDs in the seekpoint master */
        public const int MATROSKA_ID_SEEKID = 0x53AB;
        public const int MATROSKA_ID_SEEKPOSITION = 0x53AC;

        /* IDs in the cluster master */
        public const int MATROSKA_ID_CLUSTERTIMECODE = 0xE7;
        public const int MATROSKA_ID_CLUSTERPOSITION = 0xA7;
        public const int MATROSKA_ID_CLUSTERPREVSIZE = 0xAB;
        public const int MATROSKA_ID_BLOCKGROUP = 0xA0;
        public const int MATROSKA_ID_BLOCKADDITIONS = 0x75A1;
        public const int MATROSKA_ID_BLOCKMORE = 0xA6;
        public const int MATROSKA_ID_BLOCKADDID = 0xEE;
        public const int MATROSKA_ID_BLOCKADDITIONAL = 0xA5;
        public const int MATROSKA_ID_SIMPLEBLOCK = 0xA3;

        /* IDs in the blockgroup master */
        public const int MATROSKA_ID_BLOCK = 0xA1;
        public const int MATROSKA_ID_BLOCKDURATION = 0x9B;
        public const int MATROSKA_ID_BLOCKREFERENCE = 0xFB;
        public const int MATROSKA_ID_CODECSTATE = 0xA4;
        public const int MATROSKA_ID_DISCARDPADDING = 0x75A2;

        /* IDs in the attachments master */
        public const int MATROSKA_ID_ATTACHEDFILE = 0x61A7;
        public const int MATROSKA_ID_FILEDESC = 0x467E;
        public const int MATROSKA_ID_FILENAME = 0x466E;
        public const int MATROSKA_ID_FILEMIMETYPE = 0x4660;
        public const int MATROSKA_ID_FILEDATA = 0x465C;
        public const int MATROSKA_ID_FILEUID = 0x46AE;

        /* IDs in the chapters master */
        public const int MATROSKA_ID_EDITIONENTRY = 0x45B9;
        public const int MATROSKA_ID_CHAPTERATOM = 0xB6;
        public const int MATROSKA_ID_CHAPTERTIMESTART = 0x91;
        public const int MATROSKA_ID_CHAPTERTIMEEND = 0x92;
        public const int MATROSKA_ID_CHAPTERDISPLAY = 0x80;
        public const int MATROSKA_ID_CHAPSTRING = 0x85;
        public const int MATROSKA_ID_CHAPLANG = 0x437C;
        public const int MATROSKA_ID_EDITIONUID = 0x45BC;
        public const int MATROSKA_ID_EDITIONFLAGHIDDEN = 0x45BD;
        public const int MATROSKA_ID_EDITIONFLAGDEFAULT = 0x45DB;
        public const int MATROSKA_ID_EDITIONFLAGORDERED = 0x45DD;
        public const int MATROSKA_ID_CHAPTERUID = 0x73C4;
        public const int MATROSKA_ID_CHAPTERFLAGHIDDEN = 0x98;
        public const int MATROSKA_ID_CHAPTERFLAGENABLED = 0x4598;
        public const int MATROSKA_ID_CHAPTERPHYSEQUIV = 0x63C3;

        public const int EBML_MAX_DEPTH = 16;

        public const int MATROSKA_VIDEO_STEREO_PLANE_COUNT = 3;


    }
}
