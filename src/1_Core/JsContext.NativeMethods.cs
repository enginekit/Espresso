﻿//MIT, 2015-present, WinterDev, EngineKit, brezza92
//MIT, 2013, Federico Di Gregorio<fog@initd.org>

using System;
using System.Runtime.InteropServices;

namespace Espresso
{
    partial class JsContext
    {


        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr jscontext_new(int id, HandleRef engine);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern void jscontext_dispose(HandleRef engine);

        //TODO: review remove this?
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern void jscontext_force_gc();


        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern void jscontext_execute(HandleRef context,
            string str,
            string name,
            ref JsValue output);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern void jscontext_execute_script(HandleRef context, HandleRef script, ref JsValue output);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern void jscontext_get_global(HandleRef engine, ref JsValue output);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern void jscontext_get_variable(HandleRef engine,
            string name,
            ref JsValue output);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern void jscontext_set_variable(HandleRef engine,
            string name,
            ref JsValue value,
            ref JsValue output);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static internal unsafe extern void jsvalue_alloc_string(
         char* str,
         int strLen,
         ref JsValue output);

        [DllImport(JsBridge.LIB_NAME, EntryPoint = "jsvalue_alloc_string", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static internal unsafe extern void jsvalue_alloc_string2(
        char* str,
        int strLen,
        JsValue* output);


        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static internal extern void jsvalue_alloc_array(int length,
            ref JsValue output);
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static internal unsafe extern void jsvalue_alloc_array(int length,
            JsValue* output);


        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void jsvalue_buffer_get_info(HandleRef contextPtr, IntPtr jsNativeBuffer,
            ref JsValue output_bufferKind,
            ref JsValue output_buffer_len);


        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void jsvalue_buffer_get_len(HandleRef contextPtr, IntPtr jsNativeBuffer, int bufferKind, ref JsValue output);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void jsvalue_buffer_copy_buffer_data(HandleRef contextPtr, IntPtr jsNativeBuffer, int bufferKind,
            IntPtr dstMem,
            int len, ref JsValue output);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void jsvalue_buffer_write_buffer_data(HandleRef contextPtr, IntPtr jsNativeBuffer, int bufferKind,
            int dstIndex, IntPtr src, int srcLen, ref JsValue output);

        //---------------------------------------------


        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static internal extern void jscontext_invoke(HandleRef engine,
            IntPtr funcPtr,
            IntPtr thisPtr,
            ref JsValue value,
            ref JsValue output);


        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern void jscontext_get_property_names(
            HandleRef engine,
            IntPtr ptr,
            ref JsValue output);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern void jscontext_get_property_value(HandleRef engine, IntPtr ptr,
            string name,
            ref JsValue output);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern void jscontext_set_property_value(HandleRef engine,
            IntPtr ptr,
            string name,
            ref JsValue value,
            ref JsValue output);

        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern void jscontext_invoke_property(HandleRef engine, IntPtr ptr,
            string name,
            ref JsValue args,
            ref JsValue output);



        //---------------------------------------------
        //for NodeJS's NAPI
        [DllImport(JsBridge.LIB_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern void js_new_napi_env(HandleRef contextPtr, ref JsValue output);

       
    }
}
