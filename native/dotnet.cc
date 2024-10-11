#include "dotnet.h"

#include <filesystem>

#include <nethost.h>
#include <coreclr_delegates.h>
#include <hostfxr.h>

#include <spdlog/spdlog.h>

using namespace cc;

namespace
{
    hostfxr_initialize_for_dotnet_command_line_fn init_for_cmd_line_fptr;
    hostfxr_initialize_for_runtime_config_fn init_for_config_fptr;
    hostfxr_get_runtime_delegate_fn get_delegate_fptr;
    hostfxr_run_app_fn run_app_fptr;
    hostfxr_close_fn close_fptr;

    bool load_hostfxr(const std::filesystem::path& assembly_path);

    load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t* assembly);

    void* load_library(const char_t*);

    void* get_export(void*, const char*);
}

void cc::load_dotnet(InitParams* init_params, InitResult* init_result)
{
    auto path = std::filesystem::path(__argv[0]);
    path = path.parent_path();
    path.append("managed");

    auto dll_path = std::filesystem::path(path);
    dll_path.append("Game.dll");
    auto config_path = std::filesystem::path(path);
    config_path.append("Game.runtimeconfig.json");

    spdlog::debug(std::format("dll path: {}", dll_path.string()));
    spdlog::debug(std::format("dll runtimeconfig path: {}", config_path.string()));

    if (!load_hostfxr(path))
    {
        throw std::exception("Failed to load dotnet runtime");
    }

    load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = get_dotnet_load_assembly(config_path.c_str());
    assert(load_assembly_and_get_function_pointer != nullptr && "Failure: get_dotnet_load_assembly()");

    const char_t* dotnet_type = L"Game.Entry, Game";
    const char_t* dotnet_type_method = L"Init";

    typedef void (CORECLR_DELEGATE_CALLTYPE *custom_entry_point_fn)(InitParams*, InitResult*);
    custom_entry_point_fn init = nullptr;
    const int rc = load_assembly_and_get_function_pointer(
        dll_path.c_str(),
        dotnet_type,
        dotnet_type_method,
        UNMANAGEDCALLERSONLY_METHOD /*delegate_type_name*/,
        nullptr,
        (void**)&init
    );

    if (rc != 0 || init == nullptr)
    {
        throw std::exception("Failed to load dotnet runtime2");
    }

    init(init_params, init_result);
}

namespace
{
    bool load_hostfxr(const std::filesystem::path& assembly_path)
    {
        get_hostfxr_parameters params{sizeof(get_hostfxr_parameters), assembly_path.c_str(), nullptr};

        // Pre-allocate a large buffer for the path to hostfxr
        char_t buffer[MAX_PATH];
        size_t buffer_size = sizeof(buffer) / sizeof(char_t);
        int rc = get_hostfxr_path(buffer, &buffer_size, &params);
        if (rc != 0)
            return false;

        // Load hostfxr and get desired exports
        // NOTE: The .NET Runtime does not support unloading any of its native libraries. Running
        // dlclose/FreeLibrary on any .NET libraries produces undefined behavior.
        void* lib = load_library(buffer);
        init_for_cmd_line_fptr = (hostfxr_initialize_for_dotnet_command_line_fn)get_export(
            lib, "hostfxr_initialize_for_dotnet_command_line"
        );
        init_for_config_fptr = (hostfxr_initialize_for_runtime_config_fn)get_export(
            lib, "hostfxr_initialize_for_runtime_config"
        );
        get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
        run_app_fptr = (hostfxr_run_app_fn)get_export(lib, "hostfxr_run_app");
        close_fptr = (hostfxr_close_fn)get_export(lib, "hostfxr_close");

        return (init_for_config_fptr && get_delegate_fptr && close_fptr);
    }

    void* load_library(const char_t* path)
    {
        HMODULE h = ::LoadLibraryW(path);
        assert(h != nullptr);
        return (void*)h;
    }

    void* get_export(void* h, const char* name)
    {
        void* f = (void*)::GetProcAddress((HMODULE)h, name);
        assert(f != nullptr);
        return f;
    }

    load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t* assembly)
    {
        // Load .NET Core
        void* load_assembly_and_get_function_pointer = nullptr;
        hostfxr_handle cxt = nullptr;
        int rc = init_for_config_fptr(assembly, nullptr, &cxt);
        if (rc != 0 || cxt == nullptr)
        {
            spdlog::error("Failed to load dotnet runtime, init failed: {}", rc);
            close_fptr(cxt);
            return nullptr;
        }

        // Get the load assembly function pointer
        rc = get_delegate_fptr(
            cxt,
            hdt_load_assembly_and_get_function_pointer,
            &load_assembly_and_get_function_pointer
        );
        if (rc != 0 || load_assembly_and_get_function_pointer == nullptr)
            spdlog::error("Failed to load dotnet runtime, get delegate failed: {}", rc);

        close_fptr(cxt);
        return (load_assembly_and_get_function_pointer_fn)load_assembly_and_get_function_pointer;
    }
}
