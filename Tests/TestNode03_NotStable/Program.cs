﻿//
//WARNING! , experiment, from @dpwhittaker,  see https://github.com/prepare/Espresso/issues/40
//
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Espresso;
using System.Diagnostics;

namespace Test03_NotStable
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("test only!");
            ScriptRuntime3 scriptRuntime3 = new ScriptRuntime3();
            //----------
            ////test1: execute from  main thread
            //for (int i = 0; i < 100; ++i)
            //{
            //    Dictionary<string, object> scriptPars = new Dictionary<string, object>();
            //    scriptRuntime3.Execute("LibEspresso.Log('ok" + i + "');", scriptPars, (result) =>
            //        {
            //            //when finish or error this will be called
            //            var exception = result as Exception;
            //            if (exception != null)
            //            {
            //                //this is error
            //            }
            //        });
            //}
            //----------
            ////test2: execute from thread pool queue
            //for (int i = 0; i < 100; ++i)
            //{
            //    int snapNum = i;
            //    ThreadPool.QueueUserWorkItem(o =>
            //    {
            //        Dictionary<string, object> scriptPars = new Dictionary<string, object>();
            //        scriptRuntime3.Execute("LibEspresso.Log('ok" + snapNum + "');", scriptPars, (result) =>
            //        {
            //            //when finish or error this will be called
            //            var exception = result as Exception;
            //            if (exception != null)
            //            {
            //                //this is error
            //            }
            //        });
            //    });
            //}
            //----------
            //test3: async- await
            RunJsTaskWithAsyncAwait(scriptRuntime3);
            Console.ReadLine();
        }
        static async void RunJsTaskWithAsyncAwait(ScriptRuntime3 scriptRuntime3)
        {
            {
                //
                //await 
                //
                object result1 = await scriptRuntime3.ExecuteAsync("(function(){LibEspresso.Log('ok" + 1 + "');return 1;})()", new Dictionary<string, object>());
                Console.WriteLine("await result:" + result1);
                //
                object result2 = await scriptRuntime3.ExecuteAsync("(function(){LibEspresso.Log('ok" + 2 + "');return 2;})()", new Dictionary<string, object>());
                Console.WriteLine("await result:" + result2);
                //
                object result3 = await scriptRuntime3.ExecuteAsync("(function(){LibEspresso.Log('ok" + 3 + "');return 3;})()", new Dictionary<string, object>());
                Console.WriteLine("await result:" + result3);
                //
                object result4 = await scriptRuntime3.ExecuteAsync("(function(){LibEspresso.Log('ok" + 4 + "');return 4;})()", new Dictionary<string, object>());
                Console.WriteLine("await result:" + result4);

                object result5 = await scriptRuntime3.ExecuteAsync(@" 
                        const os= require('os');
                        LibEspresso.Log('myos is '+ os.arch());

                ", new Dictionary<string, object>());
                Console.WriteLine("await result:" + result5);
            }
            //----------
            {
                //async ...
                scriptRuntime3.ExecuteAsync("(function(){LibEspresso.Log('ok" + 1 + "');return 1;})()", new Dictionary<string, object>());
                //
                scriptRuntime3.ExecuteAsync("(function(){LibEspresso.Log('ok" + 2 + "');return 2;})()", new Dictionary<string, object>());
                //
                scriptRuntime3.ExecuteAsync("(function(){LibEspresso.Log('ok" + 3 + "');return 3;})()", new Dictionary<string, object>());

                //
                scriptRuntime3.ExecuteAsync("(function(){LibEspresso.Log('ok" + 4 + "');return 4;})()", new Dictionary<string, object>());

            }
        }
    }

    //-----------------



    public class ScriptRuntime3
    {
        JsEngine _engine;
        JsContext _context;
        readonly Thread _jsThread;
        readonly ConcurrentQueue<System.Action> _workQueue = new ConcurrentQueue<System.Action>();
        public ScriptRuntime3()
        {
            _jsThread = new Thread(ScriptThread);
            _jsThread.Start();
        }
        private void ScriptThread(object obj)
        {
            _workQueue.Enqueue(InitializeJsGlobals);
            NodeJsEngine.Run(Debugger.IsAttached ? new string[] { "--inspect", "hello.espr" } : new string[] { "hello.espr" },
            (eng, ctx) =>
            {
                _engine = eng;
                _context = ctx;

                JsTypeDefinition jstypedef = new JsTypeDefinition("LibEspressoClass");
                jstypedef.AddMember(new JsMethodDefinition("LoadMainSrcFile", args =>
                {
                    args.SetResult(@"
function MainLoop() {
    LibEspresso.Next();
    setImmediate(MainLoop);
}
MainLoop();");
                }));

                jstypedef.AddMember(new JsMethodDefinition("Log", args =>
                {
                    Console.WriteLine(args.GetArgAsObject(0));
                }));

                jstypedef.AddMember(new JsMethodDefinition("Next", args =>
                {
                    //call from js server
                    System.Action work;
                    if (_workQueue.TryDequeue(out work))
                    {
                        work();
                    }
                }));
                _context.RegisterTypeDefinition(jstypedef);
                _context.SetVariableFromAny("LibEspresso", _context.CreateWrapper(new object(), jstypedef));
            });
        }
        public void Execute(string script, Dictionary<string, object> processData, Action<object> doneWithResult)
        {
            _workQueue.Enqueue(() =>
            {
                foreach (var kp in processData)
                    _context.SetVariableFromAny(kp.Key, kp.Value);
                //----------------          
                object result = null;
                try
                {
                    result = _context.Execute(script);
                }
                catch (JsException ex)
                {
                    //set result as exception
                    result = ex;
                }
                //--------
                //notify result back
                doneWithResult(result);
            });
        }
        public Task<object> ExecuteAsync(string script, Dictionary<string, object> processData)
        {
            var tcs = new TaskCompletionSource<object>();
            Execute(script, processData, result =>
            {
                tcs.SetResult(result);
            });
            return tcs.Task;
        }

        void InitializeJsGlobals()
        {
            //-----------------------------------
            //1.
            //after we build nodejs in dll version
            //we will get node.dll
            //then just copy it to another name 'libespr'   

            string libEspr = @"../../../node-v15.5.1/out/Release/node.dll";
            //-----------------------------------
            //2. load libespr.dll (node.dll)
            //-----------------------------------  
            IntPtr intptr = LoadLibrary(libEspr);
            int errCode = GetLastError();
            int libesprVer = JsBridge.LibVersion;
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
        }

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        static extern IntPtr LoadLibrary(string dllname);
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        static extern int GetLastError();
    }

    public class ScriptRuntime4
    {
        JsContext _context;
        readonly Thread _jsThread;
        readonly ConcurrentQueue<System.Func<string>> _workQueue = new ConcurrentQueue<System.Func<string>>();

        public ScriptRuntime4()
        {
            _jsThread = new Thread(ScriptThread);
            _jsThread.Start();
        }

        [JsType]
        class JsWorkerNextQueue
        {

            readonly ConcurrentQueue<System.Func<string>> _workQueue;
            public JsWorkerNextQueue(ConcurrentQueue<System.Func<string>> workQueue) => _workQueue = workQueue;
            [JsMethod]
            public void Next()
            {
                //call from js server 
                if (_workQueue.TryDequeue(out System.Func<string> work))
                {
                    work();
                }
            }
            [JsMethod]
            public string NextCode()
            {
                if (_workQueue.TryDequeue(out System.Func<string> work))
                {
                    return work();
                }
                return "";
            }
        }
        [JsType]
        class MyConsole
        {
            [JsMethod]
            public void log(object msg)
            {
                if (msg != null)
                {
                    Console.WriteLine(msg.ToString());
                }
            }
        }

        private void ScriptThread(object obj)
        {

            InitializeJsGlobals();
            NodeJsEngineHelper.Run(
                ss =>
                {
                    _context = ss.Context;
                    ss.SetExternalObj("ww", new JsWorkerNextQueue(_workQueue));
                    ss.SetExternalObj("x", new MyConsole());

                    return @" 
                        function MainLoop() {
                            //ww.Next();
                            var code=ww.NextCode();
                            if(code != null){
                                eval(code);
                            }
                            setImmediate(MainLoop);
                        }
                        MainLoop();";
                });
        }
        public void Execute(string script, Dictionary<string, object> processData, Action<object> doneWithResult)
        {
            _workQueue.Enqueue(() =>
            {
                foreach (var kp in processData)
                    _context.SetVariableFromAny(kp.Key, kp.Value);
                //----------------          
                //object result = null;
                //try
                //{
                //    result = _context.Execute(script);
                //}
                //catch (JsException ex)
                //{
                //    //set result as exception
                //    result = ex;
                //}
                //--------
                //notify result back
                doneWithResult("");

                return script;
                //return "x.log(os.arch());";
            });
        }
        public Task<object> ExecuteAsync(string script, Dictionary<string, object> processData)
        {
            var tcs = new TaskCompletionSource<object>();
            Execute(script, processData, result =>
            {
                tcs.SetResult(result);
            });
            return tcs.Task;
        }

        void InitializeJsGlobals()
        {
            string libEspr = @"../../../node-v16.3.0/out/Release/node.dll";
            //-----------------------------------
            //2. load node.dll
            //-----------------------------------  
            IntPtr intptr = LoadLibrary(libEspr);
            int errCode = GetLastError();
            int libesprVer = JsBridge.LibVersion;
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
        }

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        static extern IntPtr LoadLibrary(string dllname);
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        static extern int GetLastError();
    }







}
