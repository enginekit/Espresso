﻿//MIT, 2019, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Espresso.Extension;

namespace Espresso.NodeJsApi
{
    enum napi_status
    {
        //from NodeJs
        //see => js_native_api.h 
        napi_ok,
        napi_invalid_arg,
        napi_object_expected,
        napi_string_expected,
        napi_name_expected,
        napi_function_expected,
        napi_number_expected,
        napi_boolean_expected,
        napi_array_expected,
        napi_generic_failure,
        napi_pending_exception,
        napi_cancelled,
        napi_escape_called_twice,
        napi_handle_scope_mismatch,
        napi_callback_scope_mismatch,
        napi_queue_full,
        napi_closing,
        napi_bigint_expected,
        napi_date_expected,
        napi_arraybuffer_expected,
        napi_detachable_arraybuffer_expected,
    }


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void napi_finalize(IntPtr env, IntPtr finalize_data, IntPtr finalize_hint);

    static class NodeJsApiNativeMethods
    {

        //https://nodejs.org/api/n-api.html#n_api_napi_create_array
        /// <summary>
        /// This API returns an N-API value corresponding to a JavaScript Array type. JavaScript arrays are described in Section 22.1 of the ECMAScript Language  
        /// </summary>         
        /// <param name="env"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_create_array(IntPtr env, out IntPtr result);


        //https://nodejs.org/api/n-api.html#n_api_napi_create_array_with_length
        /// <summary>
        /// This API returns an N-API value corresponding to a JavaScript Array type. The Array's length property is set to the passed-in length parameter. However, the underlying buffer is not guaranteed to be pre-allocated by the VM when the array is created. That behavior is left to the underlying VM implementation. If the buffer must be a contiguous block of memory that can be directly read and/or written via C, consider using napi_create_external_arraybuffer.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="length"> The initial length of the Array</param>
        /// <param name="result"></param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_create_array_with_length(IntPtr env, int length, out IntPtr result);


        //https://nodejs.org/api/n-api.html#n_api_napi_create_arraybuffer
        /// <summary>
        /// This API returns an N-API value corresponding to a JavaScript ArrayBuffer. ArrayBuffers are used to represent fixed-length binary data buffers. They are normally used as a backing-buffer for TypedArray objects. The ArrayBuffer allocated will have an underlying byte buffer whose size is determined by the length parameter that's passed in. The underlying buffer is optionally returned back to the caller in case the caller wants to directly manipulate the buffer. This buffer can only be written to directly from native code. To write to this buffer from JavaScript, a typed array or DataView object would need to be created.        
        /// </summary>
        /// <param name="env"></param>
        /// <param name="byte_length">The length in bytes of the array buffer to create</param>
        /// <param name="result_memPtr"> Pointer to the underlying byte buffer of the ArrayBuffer.</param>
        /// <param name="result"></param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_create_arraybuffer(IntPtr env,
            int byte_length,
            out IntPtr result_memPtr,
            out IntPtr result);


        //https://nodejs.org/api/n-api.html#n_api_napi_create_buffer
        /// <summary>
        /// This API allocates a node::Buffer object. While this is still a fully-supported data structure, in most cases using a TypedArray will suffice.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="size">Size in bytes of the underlying buffer.</param>
        /// <param name="nativeMemPtr">Raw pointer to the underlying buffer</param>
        /// <param name="result"></param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_create_buffer(IntPtr env,
           int size,
           out IntPtr nativeMemPtr,
           out IntPtr result);


        //https://nodejs.org/api/n-api.html#n_api_napi_create_buffer_copy
        /// <summary>
        /// This API allocates a node::Buffer object and initializes it with data copied from the passed-in buffer. While this is still a fully-supported data structure, in most cases using a TypedArray will suffice.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="length">Size in bytes of the input buffer (should be the same as the size of the new buffer).</param>
        /// <param name="raw_src_data"> Raw pointer to the underlying buffer to copy from.</param>
        /// <param name="result_data"> Pointer to the new Buffer's underlying data buffer.</param>
        /// <param name="result"> A napi_value representing a node::Buffer.</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_create_buffer_copy(IntPtr env,
            int length,
            IntPtr raw_src_data,
            out IntPtr result_data,
            out IntPtr result);


        //--------------------
        //https://nodejs.org/api/n-api.html#n_api_napi_create_date
        /// <summary>
        /// This API allocates a JavaScript Date object.
        /// This API does not observe leap seconds; 
        /// they are ignored, as ECMAScript aligns with POSIX time specification. 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="time"> ECMAScript time value in milliseconds since 01 January, 1970 UTC.</param>
        /// <param name="result">A napi_value representing a JavaScript Date.</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_create_date(IntPtr env,
           double time,
           out IntPtr result);



        //-------------------
        //External ...
        //https://nodejs.org/api/n-api.html#n_api_napi_create_external
        /// <summary>
        /// This API allocates a JavaScript value with external data attached to it. This is used to pass external data through JavaScript code, so it can be retrieved later by native code using napi_get_value_external.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="nativeMemPtr">Raw pointer to the external data</param>
        /// <param name="finalize_cb">Optional callback to call when the external value is being collected</param>
        /// <param name="finalize_hint"> Optional hint to pass to the finalize callback during collection.</param>
        /// <param name="result"> A napi_value representing an external value</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_create_external(IntPtr env,
           IntPtr nativeMemPtr,
           IntPtr finalize_cb,
           IntPtr finalize_hint,
           out IntPtr result);
        //        The API adds a napi_finalize callback which will be called when the JavaScript object just created is ready for garbage collection. It is similar to napi_wrap() except that:

        //    the native data cannot be retrieved later using napi_unwrap(),
        //    nor can it be removed later using napi_remove_wrap(), and
        //    the object created by the API can be used with napi_wrap().
        //The created value is not an object, and therefore does not support additional properties. It is considered a distinct value type: calling napi_typeof() with an external value yields napi_external.



        /// <summary>
        /// https://nodejs.org/api/n-api.html#n_api_napi_create_external_arraybuffer
        /// </summary>
        /// <param name="env"></param>
        /// <param name="nativeMemPtr_external_data"> Pointer to the underlying byte buffer of the ArrayBuffer</param>
        /// <param name="byte_length"> The length in bytes of the underlying buffer.</param>
        /// <param name="finalize_cb">Optional callback to call when the ArrayBuffer is being collected.</param>
        /// <param name="finalize_hint"> Optional hint to pass to the finalize callback during collection.</param>
        /// <param name="result"> A napi_value representing a JavaScript ArrayBuffer</param>
        /// <returns>Returns napi_ok if the API succeeded.</returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_create_external_arraybuffer(IntPtr env,
            IntPtr nativeMemPtr_external_data,
            int byte_length,
            IntPtr finalize_cb,
            IntPtr finalize_hint,
            out IntPtr result);

        //This API returns an N-API value corresponding to a JavaScript ArrayBuffer. The underlying byte buffer of the ArrayBuffer is externally allocated and managed. The caller must ensure that the byte buffer remains valid until the finalize callback is called.
        //The API adds a napi_finalize callback which will be called when the JavaScript object just created is ready for garbage collection. It is similar to napi_wrap() except that:
        //    the native data cannot be retrieved later using napi_unwrap(),
        //    nor can it be removed later using napi_remove_wrap(), and
        //    the object created by the API can be used with napi_wrap().
        //JavaScript ArrayBuffers are described in Section 24.1 of the ECMAScript Language Specification.


        //https://nodejs.org/api/n-api.html#n_api_napi_create_external_buffer
        /// <summary>
        /// This API allocates a node::Buffer object and initializes it with data backed by the passed in buffer. While this is still a fully-supported data structure, in most cases using a TypedArray will suffice.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="length">Size in bytes of the input buffer (should be the same as the size of the new buffer)</param>
        /// <param name="nativeMemPtr_external_data">Raw pointer to the underlying buffer to copy from</param>
        /// <param name="finalize_cb">Optional callback to call when the ArrayBuffer is being collected.</param>
        /// <param name="finalize_hint"> Optional hint to pass to the finalize callback during collection.</param>
        /// <param name="result">A napi_value representing a node::Buffer</param>
        /// <returns>Returns napi_ok if the API succeeded.</returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_create_external_buffer(IntPtr env,
           int length,
           IntPtr nativeMemPtr_external_data,
           IntPtr finalize_cb,
           IntPtr finalize_hint,
           out IntPtr result);
        // This API allocates a node::Buffer object and initializes it with data backed by the passed in buffer. While this is still a fully-supported data structure, in most cases using a TypedArray will suffice.

        //The API adds a napi_finalize callback which will be called when the JavaScript object just created is ready for garbage collection. It is similar to napi_wrap() except that:

        //    the native data cannot be retrieved later using napi_unwrap(),
        //    nor can it be removed later using napi_remove_wrap(), and
        //    the object created by the API can be used with napi_wrap().

        //For Node.js >=4 Buffers are Uint8Arrays. //***

        //-------------------
        //https://nodejs.org/api/n-api.html#n_api_napi_create_string_utf16
        /// <summary>
        /// This API creates a JavaScript String object from a UTF16-LE-encoded C string. The native string is copied.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="str">Character buffer representing a UTF16-LE-encoded string.</param>
        /// <param name="size">The length of the string in two-byte code units, or NAPI_AUTO_LENGTH if it is null-terminated.</param>
        /// <param name="result">A napi_value representing a JavaScript String</param>
        /// <returns>Returns napi_ok if the API succeeded.</returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_create_string_utf16(IntPtr env,
           IntPtr str,
           int size,
           out IntPtr result);
        //The JavaScript String type is described in Section 6.1.4 of the ECMAScript Language Specification.


        //https://nodejs.org/api/n-api.html#n_api_napi_create_string_utf8
        /// <summary>
        /// This API creates a JavaScript String object from a UTF8-encoded C string. The native string is copied.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="str">Character buffer representing a UTF8-encoded string</param>
        /// <param name="size">The length of the string in bytes, or NAPI_AUTO_LENGTH if it is null-terminated.</param>
        /// <param name="result">A napi_value representing a JavaScript String.</param>
        /// <returns>napi_ok if the API succeeded.</returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_create_string_utf8(IntPtr env,
          IntPtr str,
          int size,
          out IntPtr result);
        //The JavaScript String type is described in Section 6.1.4 of the ECMAScript Language Specification.



        //.. TODO.... add more...


        //Script execution
        //https://nodejs.org/api/n-api.html#n_api_napi_run_script
        /// <summary> 
        ///  This function executes a string of JavaScript code and returns its result with the following caveats:
        /// Unlike eval, this function does not allow the script to access the current lexical scope, and therefore also does not allow to access the module scope, meaning that pseudo-globals such as require will not be available.
        /// The script can access the global scope.Function and var declarations in the script will be added to the global object. Variable declarations made using let and const will be visible globally, but will not be added to the global object.
        /// The value of this is global within the script.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="script"> A JavaScript string containing the script to execute.</param>
        /// <param name="result">The value resulting from having executed the script.</param>
        /// <returns></returns>
        // 
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_run_script(IntPtr env,
            IntPtr script,
            out IntPtr result);


        //-----------
        /// <summary>
        /// This API represents behavior similar to invoking the typeof Operator on the object as defined in 
        /// Section 12.5.5 of the ECMAScript Language Specification.
        /// However, it has support for detecting an External value.
        /// If value has a type that is invalid, an error is returned.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="value"> The JavaScript value whose type to query.</param>
        /// <param name="result">The type of the JavaScript value</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_typeof(IntPtr env,
            IntPtr value,
            out napi_valuetype result);



        /// <summary>
        /// This API represents invoking the IsArray operation on the object as defined in 
        /// Section 7.2.2 of the ECMAScript Language Specification.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="value">The JavaScript value to check</param>
        /// <param name="result">Whether the given object is an array.</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_is_array(IntPtr env,
          IntPtr value,
          out bool result);


        /// <summary>
        /// This API checks if the Object passed in is an array buffer.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="value">The JavaScript value to check</param>
        /// <param name="result">Whether the given object is an ArrayBuffer</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_is_arraybuffer(IntPtr env,
            IntPtr value,
            out bool result);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="value">The JavaScript value to check</param>
        /// <param name="result"> Whether the given napi_value represents a node::Buffer object.</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_is_buffer(IntPtr env,
           IntPtr value,
           out bool result);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="value"></param>
        /// <param name="result">Whether the given napi_value represents a JavaScript Date object.</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_is_date(IntPtr env,
          IntPtr value,
          out bool result);

        /// <summary>
        /// This API checks if the Object passed in is an Error.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="value"></param>
        /// <param name="result">Whether the given napi_value represents an Error object</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_is_error(IntPtr env,
              IntPtr value,
              out bool result);

        /// <summary>
        /// This API checks if the Object passed in is a typed array.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="value"></param>
        /// <param name="result">Whether the given napi_value represents a TypedArray</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_is_typedarray(IntPtr env,
           IntPtr value,
           out bool result);

        /// <summary>
        /// This API checks if the Object passed in is a DataView.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="value"></param>
        /// <param name="result"> Whether the given napi_value represents a DataView.</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_is_dataview(IntPtr env,
          IntPtr value,
          out bool result);
        //-----------



        //-----------
        //Simple Asynchronous Operations
        //(see https://nodejs.org/api/n-api.html#n_api_simple_asynchronous_operations)
        //-----------
        /// <summary>
        /// This API allocates a work object that is used to execute logic asynchronously.
        /// It should be freed using napi_delete_async_work once the work is no longer required.        
        /// </summary>
        /// <param name="env"></param>
        /// <param name="async_resource">An optional object associated with the async work that will be passed to possible async_hooks init hooks.</param>
        /// <param name="async_resource_name"> Identifier for the kind of resource that is being provided 
        ///                 for diagnostic information exposed by the async_hooks API.(async_resource_name should be a null-terminated, UTF-8-encoded string.)</param>
        /// <param name="execute">The native function which should be called to execute the logic asynchronously. 
        ///             The given function is called from a worker pool thread and can execute in parallel with the main event loop thread.</param>
        /// <param name="complete">The native function which will be called when the asynchronous logic is completed or is cancelled. The given function is called from the main event loop thread</param>
        /// <param name="data">User-provided data context. This will be passed back into the execute and complete functions.</param>
        /// <param name="result"> which is the handle to the newly created async work</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_create_async_work(IntPtr env,
            /*napi_value*/  IntPtr async_resource,
            /*napi_value*/  IntPtr async_resource_name,
            /*napi_async_execute_callback*/ IntPtr execute,
            /*napi_async_execute_callback*/ IntPtr complete,
            /* void* data*/IntPtr data,
            out IntPtr result);
        //The async_resource_name identifier is provided by the user and should be representative of the type of async work being performed. It is also recommended to apply namespacing to the identifier,
        //e.g. by including the module name. See the async_hooks documentation for more information.

        /// <summary>
        /// This API frees a previously allocated work object.
        ///This API can be called even if there is a pending JavaScript exception.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="async_worker"></param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_delete_async_work(IntPtr env, IntPtr async_worker);
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_queue_async_work(IntPtr env, IntPtr async_worker);
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_cancel_async_work(IntPtr env, IntPtr async_worker);

        //-----------
        //Custom Asynchronous Operations
        //(see https://nodejs.org/dist/latest-v13.x/docs/api/n-api.html#n_api_custom_asynchronous_operations)
        //-----------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="async_resource">An optional object associated with the async work that will be passed to possible async_hooks init hooks</param>
        /// <param name="async_resource_name">Identifier for the kind of resource that is being provided for diagnostic information exposed by the async_hooks API.</param>
        /// <param name="result">he initialized async context.</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_create_async_work(IntPtr env,
            IntPtr  /*napi_value*/async_resource,
            IntPtr  /*napi_value*/async_resource_name,
            out IntPtr  /*napi_async_context* */result
            );
        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="async_resource"></param>
        /// <param name="async_resource_name"></param>
        /// <param name="result"></param>         
        /// <returns></returns>
        /// <remarks>This API can be called even if there is a pending JavaScript exception.</remarks>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_async_destroy(IntPtr env,
           IntPtr  /*napi_async_context*/async_context);


        /// <summary>
        ///This method allows a JavaScript function object to be called from a native add-on. This API is similar to napi_call_function. However, it is used to call from native code back into JavaScript after returning from an async operation (when there is no other script on the stack). 
        ///It is a fairly simple wrapper around node::MakeCallback. 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="async_context">Context for the async operation that is invoking the callback. This should normally be a value previously obtained from napi_async_init.
        /// However NULL is also allowed, which indicates the current async context (if any) is to be used for the callback.</param>
        /// <param name="recv">The this object passed to the called function</param>
        /// <param name="func">representing the JavaScript function to be invoked</param>
        /// <param name="argc">The count of elements in the argv array.</param>
        /// <param name="argv">Array of JavaScript values as napi_value representing the arguments to the function</param>
        /// <param name="result">napi_value representing the JavaScript object returned.</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_async_destroy(IntPtr env,
            IntPtr /*napi_async_context*/async_context,
            IntPtr /*napi_value*/recv,
            IntPtr /*napi_value*/func,
            int argc,
            IntPtr[] argv,
            out IntPtr result);
        //Note it is not necessary to use napi_make_callback from within a napi_async_complete_callback;
        //in that situation the callback's async context has already been set up,
        //so a direct call to napi_call_function is sufficient and appropriate. 
        //Use of the napi_make_callback function may be required when implementing custom async behavior
        //that does not use napi_create_async_work.


        /// <summary>
        /// There are cases (for example, resolving promises) where it is necessary to have the equivalent of the scope associated with a callback in 
        /// place when making certain N-API calls. 
        /// If there is no other script on the stack the napi_open_callback_scope and 
        /// napi_close_callback_scope functions can be used to open/close the required scope.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="resource_object"> An object associated with the async work that will be passed to possible async_hooks init hooks.</param>
        /// <param name="context">Context for the async operation that is invoking the callback. This should be a value previously obtained from napi_async_init.</param>
        /// <param name="result">The newly created scope.</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_open_callback_scope(IntPtr env,
            IntPtr /*napi_value*/resource_object,
            IntPtr /*napi_async_context*/context,
            out IntPtr /*napi_callback_scope* */ result
            );

        /// <summary>
        /// This API can be called even if there is a pending JavaScript exception.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="scope">The scope to be closed.</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_close_callback_scope(IntPtr env,
           IntPtr /*napi_callback_scope*/scope);
        //-----------


        //-----------
        //Promises
        //-----------

        //https://nodejs.org/dist/latest-v13.x/docs/api/n-api.html#n_api_napi_create_promise
        /// <summary>
        /// This API creates a deferred object and a JavaScript promise.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="deferred"> A newly created deferred object which can later be passed to napi_resolve_deferred() or napi_reject_deferred() to resolve resp. reject the associated promise.</param>
        /// <param name="promise">The JavaScript promise associated with the deferred object</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_create_promise(IntPtr env,
            out IntPtr deferred,
            out IntPtr promise);


        /// <summary>
        /// This API resolves a JavaScript promise by way of the deferred object with which it is associated.Thus, it can only be used to resolve JavaScript promises for which the corresponding deferred object is available.This effectively means that the promise must have been created using napi_create_promise() and the deferred object returned from that call must have been retained in order to be passed to this API.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="deferred">The deferred object whose associated promise to resolve.</param>
        /// <param name="resolution">The value with which to resolve the promise.</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_resolve_deferred(IntPtr env,
           IntPtr deferred,
           IntPtr resolution);
        //The deferred object is freed upon successful completion.


        /// <summary>
        ///  This API rejects a JavaScript promise by way of the deferred object with which it is associated.
        ///  Thus, it can only be used to reject JavaScript promises for which the corresponding deferred object is available.
        ///  This effectively means that the promise must have been created using napi_create_promise() and 
        ///  the deferred object returned from that call must have been retained in order to be passed to this API.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="deferred"> The deferred object whose associated promise to resolve.</param>
        /// <param name="rejection">The value with which to reject the promise.</param>
        /// <returns></returns> 
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_reject_deferred(IntPtr env,
            IntPtr deferred,
            IntPtr rejection);
        //The deferred object is freed upon successful completion.


        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="promise">The promise to examine</param>
        /// <param name="result">is_promise: Flag indicating whether promise is a native promise object (that is, a promise object created by the underlying engine)</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_reject_deferred(IntPtr env,
            IntPtr promise,
            out bool result);




        //-----------
        //https://nodejs.org/dist/latest-v13.x/docs/api/n-api.html#n_api_napi_get_node_version
        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="version">A pointer to version information for Node.js itself.</param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_get_node_version(IntPtr env,
           out IntPtr version);
        //typedef struct {
        //  uint32_t major;
        //        uint32_t minor;
        //        uint32_t patch;
        //        const char* release;
        //    }
        //    napi_node_version; 
        //napi_status napi_get_node_version(napi_env env,
        //                                  const napi_node_version** version);


        //https://nodejs.org/dist/latest-v13.x/docs/api/n-api.html#n_api_napi_get_version
        /// <summary>
        ///  This API returns the highest N-API version supported by the Node.js runtime.
        ///  N-API is planned to be additive such that newer releases of Node.js may support additional API functions. 
        ///  In order to allow an addon to use a newer function when running with versions of Node.js that support it, 
        ///  while providing fallback behavior when running with Node.js versions that don't support it:
        /// </summary>
        /// <param name="env"></param>
        /// <param name="result"></param>
        /// <returns></returns>

        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_get_version(IntPtr env,
            out uint result);

        //Call napi_get_version() to determine if the API is available.
        //If available, dynamically load a pointer to the function using uv_dlsym().
        //Use the dynamically loaded pointer to invoke the function.
        //If the function is not available, provide an alternate implementation that does not use the function.



        //--------------------------------------------
        //libuv event loop
        //The current libuv loop instance.
        //https://nodejs.org/dist/latest-v13.x/docs/api/n-api.html#n_api_libuv_event_loop
        /// <summary>
        /// N-API provides a function for getting the current event loop associated with a specific napi_env.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        [DllImport(JsBridge.LIB_NAME)]
        internal static extern napi_status napi_get_uv_event_loop(IntPtr env,
            out IntPtr loop);

    }
    delegate void napi_async_execute_callback(IntPtr env, IntPtr data/* void* */);
    delegate void napi_async_complete_callback(IntPtr env, napi_status status, IntPtr data/* void* */);

    [StructLayout(LayoutKind.Sequential)]
    public struct napi_node_version
    {
        public uint major;
        public uint minor;
        public uint patch;
        public string release;
    }
}
