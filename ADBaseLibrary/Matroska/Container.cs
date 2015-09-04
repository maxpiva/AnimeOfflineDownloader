using System.Collections.Generic;
using ADBaseLibrary.Matroska.Objects;

namespace ADBaseLibrary.Matroska
{
    public class Container : EbmlMaster
    {
        internal static List<EbmlGeneric> General = new List<EbmlGeneric>
            {
                new EbmlBinary { Id= Ids.EBML_ID_VOID },
                new EbmlBinary { Id= Ids.EBML_ID_CRC32 }
            };

        internal static List<EbmlGeneric> Header = new List<EbmlGeneric>
            {
                new EbmlUint {Id = Ids.EBML_ID_EBMLREADVERSION},
                new EbmlUint {Id = Ids.EBML_ID_EBMLMAXSIZELENGTH},
                new EbmlUint {Id = Ids.EBML_ID_EBMLMAXIDLENGTH},
                new EbmlAscii {Id = Ids.EBML_ID_DOCTYPE},
                new EbmlUint {Id = Ids.EBML_ID_DOCTYPEREADVERSION},
                new EbmlUint {Id = Ids.EBML_ID_EBMLVERSION},
                new EbmlUint {Id = Ids.EBML_ID_DOCTYPEVERSION}
            };


        internal static List<EbmlGeneric> SeekHead = new List<EbmlGeneric>
            {
                new EbmlUint {Id = Ids.MATROSKA_ID_SEEKID},
                new EbmlUint {Id = Ids.MATROSKA_ID_SEEKPOSITION}
            };

        internal static List<EbmlGeneric> SeekEntry = new List<EbmlGeneric>
            {
                new EbmlMaster {Id = Ids.MATROSKA_ID_SEEKENTRY, Available = SeekHead}
            };

        internal static List<EbmlGeneric> Info = new List <EbmlGeneric>
            {
                new EbmlUint {Id = Ids.MATROSKA_ID_TIMECODESCALE},
                new EbmlFloat {Id = Ids.MATROSKA_ID_DURATION},
                new EbmlUtf8 {Id = Ids.MATROSKA_ID_TITLE},
                new EbmlUtf8 {Id = Ids.MATROSKA_ID_WRITINGAPP},
                new EbmlUtf8 {Id = Ids.MATROSKA_ID_MUXINGAPP},
                new EbmlBinary {Id = Ids.MATROSKA_ID_DATEUTC},
                new EbmlBinary {Id = Ids.MATROSKA_ID_SEGMENTUID},
            };

        internal static List<EbmlGeneric> TrackVideo  = new List<EbmlGeneric>
            {
                new EbmlFloat { Id = Ids.MATROSKA_ID_VIDEOFRAMERATE },
                new EbmlUint { Id = Ids.MATROSKA_ID_VIDEODISPLAYWIDTH },
                new EbmlUint { Id = Ids.MATROSKA_ID_VIDEODISPLAYHEIGHT },
                new EbmlUint { Id = Ids.MATROSKA_ID_VIDEOPIXELWIDTH },
                new EbmlUint { Id = Ids.MATROSKA_ID_VIDEOPIXELHEIGHT },
                new EbmlBinary { Id = Ids.MATROSKA_ID_VIDEOCOLORSPACE },
                new EbmlUint { Id = Ids.MATROSKA_ID_VIDEOALPHAMODE },
                new EbmlUint { Id = Ids.MATROSKA_ID_VIDEOPIXELCROPB },
                new EbmlUint { Id = Ids.MATROSKA_ID_VIDEOPIXELCROPT },
                new EbmlUint { Id = Ids.MATROSKA_ID_VIDEOPIXELCROPL },
                new EbmlUint { Id = Ids.MATROSKA_ID_VIDEOPIXELCROPR },
                new EbmlUint { Id = Ids.MATROSKA_ID_VIDEODISPLAYUNIT },
                new EbmlUint { Id = Ids.MATROSKA_ID_VIDEOFLAGINTERLACED },
                new EbmlUint { Id = Ids.MATROSKA_ID_VIDEOSTEREOMODE },
                new EbmlUint { Id = Ids.MATROSKA_ID_VIDEOASPECTRATIO },

            };

        internal static List<EbmlGeneric> TrackAudio = new List<EbmlGeneric>
            {
                new EbmlFloat {Id = Ids.MATROSKA_ID_AUDIOSAMPLINGFREQ},
                new EbmlFloat {Id = Ids.MATROSKA_ID_AUDIOOUTSAMPLINGFREQ},
                new EbmlUint {Id = Ids.MATROSKA_ID_AUDIOBITDEPTH},
                new EbmlUint {Id = Ids.MATROSKA_ID_AUDIOCHANNELS},
            };

        internal static List<EbmlGeneric> TrackEncodingCompression = new List<EbmlGeneric>
            {
                new EbmlUint { Id = Ids.MATROSKA_ID_ENCODINGCOMPALGO },
                new EbmlBinary { Id = Ids.MATROSKA_ID_ENCODINGCOMPSETTINGS },
            };

        internal static List<EbmlGeneric> TrackEncodingEncryption = new List<EbmlGeneric>
            {
                new EbmlUint {Id = Ids.MATROSKA_ID_ENCODINGENCALGO},
                new EbmlBinary {Id = Ids.MATROSKA_ID_ENCODINGENCKEYID},
                new EbmlBinary {Id = Ids.MATROSKA_ID_ENCODINGENCAESSETTINGS},
                new EbmlUint {Id = Ids.MATROSKA_ID_ENCODINGSIGALGO},
                new EbmlUint {Id = Ids.MATROSKA_ID_ENCODINGSIGHASHALGO},
                new EbmlBinary {Id = Ids.MATROSKA_ID_ENCODINGSIGKEYID},
                new EbmlBinary {Id = Ids.MATROSKA_ID_ENCODINGSIGNATURE},
            };

        internal static List<EbmlGeneric> TrackEncoding = new List<EbmlGeneric>
            {
                new EbmlUint {Id = Ids.MATROSKA_ID_ENCODINGSCOPE},
                new EbmlUint {Id = Ids.MATROSKA_ID_ENCODINGTYPE},
                new EbmlMaster { Id = Ids.MATROSKA_ID_ENCODINGCOMPRESSION,Available = TrackEncodingCompression },
                new EbmlMaster { Id = Ids.MATROSKA_ID_ENCODINGENCRYPTION, Available = TrackEncodingEncryption },
                new EbmlUint {Id = Ids.MATROSKA_ID_ENCODINGORDER},
            };
        internal static List<EbmlGeneric> TrackEncodings = new List<EbmlGeneric>
            {
                new EbmlMaster {Id = Ids.MATROSKA_ID_TRACKCONTENTENCODING, Available = TrackEncoding}
            };
        internal static List<EbmlGeneric> TrackPlane = new List<EbmlGeneric>
            {
                new EbmlUint {Id = Ids.MATROSKA_ID_TRACKPLANEUID},
                new EbmlUint {Id = Ids.MATROSKA_ID_TRACKPLANETYPE},
            };
        internal static List<EbmlGeneric> TrackCombinePlanes = new List<EbmlGeneric>
            {
                new EbmlMaster {Id = Ids.MATROSKA_ID_TRACKPLANE, Available = TrackPlane}
            };

        internal static List<EbmlGeneric> TrackOperation  = new List<EbmlGeneric>
            {
                new EbmlMaster {Id = Ids.MATROSKA_ID_TRACKCOMBINEPLANES, Available =  TrackCombinePlanes}
            };

        internal static List<EbmlGeneric> Track = new List<EbmlGeneric>
            {
                new EbmlUint {Id = Ids.MATROSKA_ID_TRACKNUMBER},
                new EbmlUtf8 {Id = Ids.MATROSKA_ID_TRACKNAME},
                new EbmlUint {Id = Ids.MATROSKA_ID_TRACKUID},
                new EbmlUint {Id = Ids.MATROSKA_ID_TRACKTYPE},
                new EbmlAscii {Id = Ids.MATROSKA_ID_CODECID},
                new EbmlBinary {Id = Ids.MATROSKA_ID_CODECPRIVATE},
                new EbmlUint {Id = Ids.MATROSKA_ID_CODECDELAY},
                new EbmlUtf8 {Id = Ids.MATROSKA_ID_TRACKLANGUAGE},
                new EbmlUint {Id = Ids.MATROSKA_ID_TRACKDEFAULTDURATION},
                new EbmlFloat {Id = Ids.MATROSKA_ID_TRACKTIMECODESCALE},
                new EbmlUint {Id = Ids.MATROSKA_ID_TRACKFLAGDEFAULT},
                new EbmlUint {Id = Ids.MATROSKA_ID_TRACKFLAGFORCED},
                new EbmlMaster {Id = Ids.MATROSKA_ID_TRACKVIDEO, Available = TrackVideo},
                new EbmlMaster {Id = Ids.MATROSKA_ID_TRACKAUDIO, Available = TrackAudio},
                new EbmlMaster {Id = Ids.MATROSKA_ID_TRACKOPERATION, Available = TrackOperation},
                new EbmlMaster {Id = Ids.MATROSKA_ID_TRACKCONTENTENCODINGS, Available = TrackEncodings},
                new EbmlUint {Id = Ids.MATROSKA_ID_TRACKMAXBLKADDID},
                new EbmlUint {Id = Ids.MATROSKA_ID_SEEKPREROLL},
                new EbmlUint {Id = Ids.MATROSKA_ID_TRACKFLAGENABLED},
                new EbmlUint {Id = Ids.MATROSKA_ID_TRACKFLAGLACING},
                new EbmlUtf8 {Id = Ids.MATROSKA_ID_CODECNAME},
                new EbmlUint {Id = Ids.MATROSKA_ID_CODECDECODEALL},
                new EbmlAscii {Id = Ids.MATROSKA_ID_CODECINFOURL},
                new EbmlAscii {Id = Ids.MATROSKA_ID_CODECDOWNLOADURL},
                new EbmlUint {Id = Ids.MATROSKA_ID_TRACKMINCACHE},
                new EbmlUint {Id = Ids.MATROSKA_ID_TRACKMAXCACHE},
            };

        internal static List<EbmlGeneric> TrackEntry = new List<EbmlGeneric>
            {
                new EbmlMaster {Id = Ids.MATROSKA_ID_TRACKENTRY, Available = Track}
            };

        internal static List<EbmlGeneric> Attachment = new List<EbmlGeneric>
            {
                new EbmlUint {Id = Ids.MATROSKA_ID_FILEUID},
                new EbmlUtf8 {Id = Ids.MATROSKA_ID_FILENAME},
                new EbmlAscii {Id = Ids.MATROSKA_ID_FILEMIMETYPE},
                new EbmlBinary {Id = Ids.MATROSKA_ID_FILEDATA,},
                new EbmlUtf8 {Id = Ids.MATROSKA_ID_FILEDESC},
            };

        internal static List<EbmlGeneric> Attachments = new List<EbmlGeneric>
            {
                new EbmlMaster {Id = Ids.MATROSKA_ID_ATTACHEDFILE, Available = Attachment }
            };
        internal static List<EbmlGeneric> ChapterDisplay = new List<EbmlGeneric>
            {
                new EbmlUtf8 {Id = Ids.MATROSKA_ID_CHAPSTRING},
                new EbmlAscii {Id = Ids.MATROSKA_ID_CHAPLANG},
            };

        internal static List<EbmlGeneric> ChapterEntry = new List<EbmlGeneric>
            {
                new EbmlUint {Id = Ids.MATROSKA_ID_CHAPTERTIMESTART},
                new EbmlUint {Id = Ids.MATROSKA_ID_CHAPTERTIMEEND},
                new EbmlUint {Id = Ids.MATROSKA_ID_CHAPTERUID},
                new EbmlMaster {Id = Ids.MATROSKA_ID_CHAPTERDISPLAY, Available = ChapterDisplay },
                new EbmlUint {Id = Ids.MATROSKA_ID_CHAPTERFLAGHIDDEN},
                new EbmlUint {Id = Ids.MATROSKA_ID_CHAPTERFLAGENABLED},
                new EbmlUint {Id = Ids.MATROSKA_ID_CHAPTERPHYSEQUIV},
                new EbmlMaster {Id = Ids.MATROSKA_ID_CHAPTERATOM, Available = ChapterEntry },
            };

        internal static List<EbmlGeneric> Chapter = new List<EbmlGeneric>
            {
                new EbmlMaster {Id = Ids.MATROSKA_ID_CHAPTERATOM, Available = ChapterEntry },
                new EbmlUint {Id = Ids.MATROSKA_ID_EDITIONUID},
                new EbmlUint {Id = Ids.MATROSKA_ID_EDITIONFLAGHIDDEN},
                new EbmlUint {Id = Ids.MATROSKA_ID_EDITIONFLAGDEFAULT},
                new EbmlUint {Id = Ids.MATROSKA_ID_EDITIONFLAGORDERED},
            };

        internal static List<EbmlGeneric> Chapters = new List<EbmlGeneric>
            {
               new EbmlMaster {Id = Ids.MATROSKA_ID_EDITIONENTRY, Available = Chapter }
            };

        internal static List<EbmlGeneric> IndexPos = new List<EbmlGeneric>
            {
                new EbmlUint {Id = Ids.MATROSKA_ID_CUETRACK, Weight = 1},
                new EbmlUint {Id = Ids.MATROSKA_ID_CUECLUSTERPOSITION, Weight = 2},
                new EbmlUint {Id = Ids.MATROSKA_ID_CUERELATIVEPOSITION, Weight = 3},
                new EbmlUint {Id = Ids.MATROSKA_ID_CUEDURATION, Weight=4},
                new EbmlUint {Id = Ids.MATROSKA_ID_CUEBLOCKNUMBER, Weight=5},
            };

        internal static List<EbmlGeneric> IndexEntry = new List<EbmlGeneric>
            {
                new EbmlUint {Id = Ids.MATROSKA_ID_CUETIME, Weight=1},
                new EbmlMaster {Id = Ids.MATROSKA_ID_CUETRACKPOSITION, Available = IndexPos,Weight=1 }
            };

        internal static List<EbmlGeneric> Index = new List<EbmlGeneric>
            {
                new EbmlMaster {Id = Ids.MATROSKA_ID_POINTENTRY, Available = IndexEntry }
            };

        internal static List<EbmlGeneric> SimpleTag = new List<EbmlGeneric>
            {
                new EbmlUtf8 {Id = Ids.MATROSKA_ID_TAGNAME},
                new EbmlUtf8 {Id = Ids.MATROSKA_ID_TAGSTRING},
                new EbmlAscii {Id = Ids.MATROSKA_ID_TAGLANG},
                new EbmlUint {Id = Ids.MATROSKA_ID_TAGDEFAULT},
                new EbmlUint {Id = Ids.MATROSKA_ID_TAGDEFAULT_BUG},
                new EbmlMaster {Id = Ids.MATROSKA_ID_SIMPLETAG, Available = SimpleTag }
            };

        internal static List<EbmlGeneric> TagTargets = new List<EbmlGeneric>
            {
                new EbmlAscii {Id = Ids.MATROSKA_ID_TAGTARGETS_TYPE},
                new EbmlUint {Id = Ids.MATROSKA_ID_TAGTARGETS_TYPEVALUE},
                new EbmlUint {Id = Ids.MATROSKA_ID_TAGTARGETS_TRACKUID},
                new EbmlUint {Id = Ids.MATROSKA_ID_TAGTARGETS_CHAPTERUID},
                new EbmlUint {Id = Ids.MATROSKA_ID_TAGTARGETS_ATTACHUID},
            };

        internal static List<EbmlGeneric> Tag = new List<EbmlGeneric>
            {
                new EbmlMaster {Id = Ids.MATROSKA_ID_SIMPLETAG, Available = SimpleTag },
                new EbmlMaster {Id = Ids.MATROSKA_ID_TAGTARGETS, Available = TagTargets }
            };

        internal static List<EbmlGeneric> Tags = new List<EbmlGeneric>
            {
                new EbmlMaster {Id = Ids.MATROSKA_ID_TAG, Available = Tag },
            };

        internal static List<EbmlGeneric> Blockmore = new List<EbmlGeneric>
            {
                new EbmlUint {Id = Ids.MATROSKA_ID_BLOCKADDID},
                new EbmlBinary {Id = Ids.MATROSKA_ID_BLOCKADDITIONAL},
            };

        internal static List<EbmlGeneric> BlockAdditions = new List<EbmlGeneric>
            {
                new EbmlMaster {Id = Ids.MATROSKA_ID_BLOCKMORE, Available = Blockmore },
            };


        internal static List<EbmlGeneric> BlockGroup = new List<EbmlGeneric>
            {
                new EbmlVirtualBinary {Id = Ids.MATROSKA_ID_BLOCK,Weight=-1},
                new EbmlMaster {Id = Ids.MATROSKA_ID_BLOCKADDITIONS, Available = BlockAdditions,Weight=-1 },
                new EbmlVirtualBinary {Id = Ids.MATROSKA_ID_SIMPLEBLOCK,Weight=-1},
                new EbmlUint {Id = Ids.MATROSKA_ID_BLOCKDURATION,Weight=-1},
                new EbmlSint {Id = Ids.MATROSKA_ID_DISCARDPADDING,Weight=-1},
                new EbmlSint {Id = Ids.MATROSKA_ID_BLOCKREFERENCE,Weight=-1},
                new EbmlBinary {Id = Ids.MATROSKA_ID_CODECSTATE,Weight=-1},
            };

        internal static List<EbmlGeneric> Cluster = new List<EbmlGeneric>
            {
                new EbmlUint {Id = Ids.MATROSKA_ID_CLUSTERTIMECODE, Weight=-1},
                new EbmlMaster {Id = Ids.MATROSKA_ID_BLOCKGROUP, Available = BlockGroup, Weight=-1 },
                new EbmlVirtualBinary {Id = Ids.MATROSKA_ID_SIMPLEBLOCK, Weight=-1},
                new EbmlUint {Id = Ids.MATROSKA_ID_CLUSTERPOSITION, Weight=-1},
                new EbmlUint {Id = Ids.MATROSKA_ID_CLUSTERPREVSIZE, Weight=-1},
            };

        internal static List<EbmlGeneric> Segment = new List<EbmlGeneric>
            {
                new EbmlMaster {Id = Ids.MATROSKA_ID_SEEKHEAD, Available = SeekEntry , Weight=1},
                new EbmlMaster {Id = Ids.MATROSKA_ID_INFO, Available = Info , Weight=2},
                new EbmlMaster {Id = Ids.MATROSKA_ID_TRACKS, Available = TrackEntry , Weight=3},
                new EbmlMaster {Id = Ids.MATROSKA_ID_CHAPTERS, Available = Chapters , Weight=4},
                new EbmlMaster {Id = Ids.MATROSKA_ID_ATTACHMENTS, Available = Attachments , Weight=5},
                new EbmlMaster {Id = Ids.MATROSKA_ID_TAGS, Available = Tags, Weight=6 },
                new EbmlMaster {Id = Ids.MATROSKA_ID_CUES, Available = Index , Weight=7},
                new EbmlMaster {Id = Ids.MATROSKA_ID_CLUSTER, Available = Cluster , Weight=8},
            };

        internal static List<EbmlGeneric> BaseContainer = new List<EbmlGeneric>
            {
                new EbmlMaster { Id = Ids.EBML_ID_HEADER, Available = Header , Weight=1},
                new EbmlMaster { Id = Ids.MATROSKA_ID_SEGMENT, Available = Segment , Weight=2},
            };


        public Container()
        {
            Available = BaseContainer;
            NoHead = true;
        }

    }
}             