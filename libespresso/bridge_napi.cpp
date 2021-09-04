//MIT, 2019, WinterDev
#include <js_native_api.h>
#include <iostream>
#include "espresso.h"

#include "node_api.h"
#include "env.h"
#include "env-inl.h"
#include "js_native_api_v8.h"

using namespace v8;
  

//---------------
//COPY from nodejs project's node_api.cc
//change node_napi_env__ to node_napi_env__2
// node_napi impl
struct node_napi_env__2 : public napi_env__ {
  explicit node_napi_env__2(v8::Local<v8::Context> context)
      : napi_env__(context) {
    CHECK_NOT_NULL(node_env());
  }

  inline node::Environment* node_env() const {
    return node::Environment::GetCurrent(context());
  }

  bool can_call_into_js() const override {
    return node_env()->can_call_into_js();
  }

  v8::Maybe<bool> mark_arraybuffer_as_untransferable(
      v8::Local<v8::ArrayBuffer> ab) const override {
    return ab->SetPrivate(context(),
                          node_env()->untransferable_object_private_symbol(),
                          v8::True(isolate));
  }

  void CallFinalizer(napi_finalize cb, void* data, void* hint) override {
    napi_env env = static_cast<napi_env>(this);
    node_env()->SetImmediate([=](node::Environment* node_env) {
      v8::HandleScope handle_scope(env->isolate);
      v8::Context::Scope context_scope(env->context());
      env->CallIntoModule([&](napi_env env) { cb(env, data, hint); });
    });
  }
};
	
typedef node_napi_env__2* node_napi_env;

static inline napi_env NewEnv(v8::Local<v8::Context> context) {
  node_napi_env result;   

  result = new node_napi_env__2(context);
  // TODO(addaleax): There was previously code that tried to delete the
  // napi_env when its v8::Context was garbage collected;
  // However, as long as N-API addons using this napi_env are in place,
  // the Context needs to be accessible and alive.
  // Ideally, we'd want an on-addon-unload hook that takes care of this
  // once all N-API addons using this napi_env are unloaded.
  // For now, a per-Environment cleanup hook is the best we can do.
  result->node_env()->AddCleanupHook(
      [](void* arg) { static_cast<napi_env>(arg)->Unref(); },
      static_cast<void*>(result));

  return result;
}
//---------------

extern "C" {
EXPORT void CALLCONV
js_new_napi_env(JsContext* contextPtr, jsvalue* output) {
  
  auto ctx = contextPtr->isolate_->GetCurrentContext();
  napi_env env = NewEnv(ctx);      
  output->ptr = env;
  output->type = JSVALUE_TYPE_WRAPPED;
}
}