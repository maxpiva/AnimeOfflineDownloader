package {
	
	import flash.display.*;
	import flash.errors.*;
	import flash.events.*;
	import flash.net.*;
	import flash.system.*;
	import flash.utils.*;



    public class DecryptorLoader extends Object
    {
        private static const PATH:String = "http://players.edgesuite.net/flash/analytics/qos/v1.3.1/data.swf";
        private static var _loaded:Boolean = false;
        private static var _functions:Object;

        public static function get loaded() : Boolean
        {
            return _loaded;
        }
        public function DecryptorLoader()
        {
            return;
        }

        public static function get Functions() : Object
        {
            return _functions;
        }

        public static function load() : void
        {
            var loader:Loader = new Loader();
            var onComplete:Function = function (event:Event) : void
            {
                loader.removeEventListener(Event.COMPLETE, onComplete);
                _functions = event.target.content;
				_loaded= true;
                return;
            }
            var ctxt:* = new LoaderContext(true, ApplicationDomain.currentDomain);
            var req:* = new URLRequest(PATH);
            loader.contentLoaderInfo.addEventListener(Event.COMPLETE, onComplete);
            loader.load(req, ctxt);
            return;
        }

    }
	
}
