using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Msie;
using JavaScriptEngineSwitcher.V8;


namespace JCarrollOnlineV2.App_Start
{
    public class JsEngineSwitcherConfig
    {
        public static void Configure(JsEngineSwitcher engineSwitcher)
        {
            engineSwitcher.EngineFactories
                .AddMsie(new MsieSettings
                {
                    UseEcmaScript5Polyfill = true,
                    UseJson2Library = true,
                    EngineMode = JsEngineMode.Auto,
                    EnableDebugging = true
                })
                .AddV8();

            engineSwitcher.DefaultEngineName = MsieJsEngine.EngineName;
        }
    }
}