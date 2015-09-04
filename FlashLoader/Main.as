package
{
    import flash.display.MovieClip;
	import flash.external.ExternalInterface;
	import flash.utils.ByteArray;
	
    public class Main extends MovieClip
    {
        public function Main()
        {
           DecryptorLoader.load();
		   ExternalInterface.addCallback("decrypt", function (datas:String,keys:String):String
		   {
				if (!DecryptorLoader.loaded)
				{
				   return "";
				}
				var data_array:Array = datas.split(",");
				var key_array:Array = keys.split(",");
				var result:String ="";
				for (var i = 0; i<data_array.length; i++)
				{

					var dtab:ByteArray = Base64.decodeToByteArray(data_array[i]);
					var dkey:ByteArray = Base64.decodeToByteArray(key_array[i]);
					if (i>0)
						result+=",";
					var res:ByteArray = DecryptorLoader.Functions.tagDecrypt(dkey, dtab);
					result +=Base64.encodeByteArray(res);
				}
				return result;
		   });
        }
    }
}
 