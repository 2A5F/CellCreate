cmake_minimum_required(VERSION 3.29)
project(cc)

include(FetchContent)

set(CMAKE_CXX_STANDARD 23)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_EXPORT_COMPILE_COMMANDS ON)
set(VCPKG_LIBRARY_LINKAGE static)

FetchContent_Declare(
        deno
        URL https://github.com/denoland/deno/releases/download/v2.0.0/deno-x86_64-pc-windows-msvc.zip
)

FetchContent_Declare(
        dotnet-host
        URL https://www.nuget.org/api/v2/package/Microsoft.NETCore.App.Host.win-x64/8.0.10
)

FetchContent_Declare(
        shader-re-tool
        URL https://github.com/2A5F/Coplt.ShaderReflections/releases/download/0.1.0/Coplt.ShaderReflections.Dxc.7z
)

FetchContent_MakeAvailable(deno)
FetchContent_MakeAvailable(dotnet-host)
FetchContent_MakeAvailable(shader-re-tool)

set(SPDLOG_FMT_EXTERNAL ON)
set(SDL_SHARED OFF)
set(SDL_STATIC ON)
set(SDL_VULKAN OFF)
set(SDL_OPENGLES OFF)
set(SDL_OPENGL OFF)
set(SDL_OFFSCREEN OFF)
set(SDL_METAL OFF)
set(SDL_DIRECTX OFF)
set(SDL_CAMERA OFF)
set(SDL_RENDER OFF)
set(SDL_HAPTIC OFF)
set(SDL_SENSOR OFF)
set(SDL_POWER OFF)
set(SDL_HIDAPI OFF)
set(SDL_DIALOG OFF)

add_definitions(-DUNICODE)
add_definitions(-D_UNICODE)
add_definitions(-DCPPTRACE_STATIC_DEFINE)
add_definitions(-DASIO_HAS_CO_AWAIT)
add_definitions(-DGLM_FORCE_INLINE)
add_definitions(-DGLM_FORCE_ALIGNED_GENTYPES)
add_definitions(-DGLM_FORCE_DEFAULT_ALIGNED_GENTYPES)
add_definitions(-DGLM_FORCE_INTRINSICS)
add_definitions(-DGLM_FORCE_SWIZZLE)
add_definitions(-DGLM_FORCE_LEFT_HANDED)
add_definitions(-DGLM_FORCE_DEPTH_ZERO_TO_ONE)
add_definitions(-DGLM_FORCE_UNRESTRICTED_GENTYPE)
add_definitions(-DGLM_FORCE_QUAT_DATA_XYZW)
add_definitions(-DCO_SRC)

add_executable(${PROJECT_NAME} native/main.cpp
        third_party/D3D12MemoryAllocator/include/D3D12MemAlloc.h
        third_party/D3D12MemoryAllocator/src/D3D12MemAlloc.cpp
        native/utils/console_utils.h
        native/utils/console_utils.cc
        native/utils/error.h
        native/dotnet.cc
        native/dotnet.h
        native/ffi/FFI.h
        native/defines.h
        native/types.h
        native/App.cc
        native/App.h
        native/Object.h
        native/ffi/Window.h
        native/window/Window.cc
        native/window/Window.h
        native/ffi/App.h
        native/ffi/Rendering.h
        native/rendering/Rendering.cc
        native/rendering/Rendering.h
)

if (WIN32)
    target_link_options(${PROJECT_NAME} PRIVATE "/SUBSYSTEM:WINDOWS")
    target_link_options(${PROJECT_NAME} PRIVATE "/ENTRY:mainCRTStartup")
endif ()

file(GLOB SHADERS_PATH "content/shaders")
file(GLOB SHADERS_FILES "content/shaders/**/*.meta")

add_subdirectory(third_party/mimalloc EXCLUDE_FROM_ALL)
add_subdirectory(third_party/SDL EXCLUDE_FROM_ALL)
add_subdirectory(third_party/D3D12MemoryAllocator EXCLUDE_FROM_ALL)
add_subdirectory(third_party/cpptrace EXCLUDE_FROM_ALL)
find_package(cppwinrt CONFIG REQUIRED)
find_package(directx-headers CONFIG REQUIRED)
find_package(directx12-agility CONFIG REQUIRED)
find_package(directxtk12 CONFIG REQUIRED)
find_package(directx-dxc CONFIG REQUIRED)
find_package(glm CONFIG REQUIRED)
find_package(fmt CONFIG REQUIRED)
find_package(spdlog CONFIG REQUIRED)
find_package(stduuid CONFIG REQUIRED)
find_package(unordered_dense CONFIG REQUIRED)

target_include_directories(${PROJECT_NAME} PRIVATE ${PACKAGE_PREFIX_DIR}/include)
target_link_directories(${PROJECT_NAME} PRIVATE ${PACKAGE_PREFIX_DIR}/lib)
target_include_directories(${PROJECT_NAME} PRIVATE third_party/D3D12MemoryAllocator/include)
target_include_directories(${PROJECT_NAME} PRIVATE ${dotnet-host_SOURCE_DIR}/runtimes/win-x64/native)
target_link_directories(${PROJECT_NAME} PRIVATE ${dotnet-host_SOURCE_DIR}/runtimes/win-x64/native)
target_link_libraries(${PROJECT_NAME} PRIVATE mimalloc)
target_link_libraries(${PROJECT_NAME} PRIVATE SDL3::SDL3)
target_link_libraries(${PROJECT_NAME} PRIVATE fmt::fmt-header-only)
target_link_libraries(${PROJECT_NAME} PRIVATE spdlog::spdlog_header_only)
target_link_libraries(${PROJECT_NAME} PRIVATE stduuid)
target_link_libraries(${PROJECT_NAME} PRIVATE Microsoft::CppWinRT RuntimeObject dwmapi dxgi.lib)
target_link_libraries(${PROJECT_NAME} PRIVATE Microsoft::DirectX-Headers Microsoft::DirectX-Guids Microsoft::DirectX12-Agility)
target_link_libraries(${PROJECT_NAME} PRIVATE Microsoft::DirectXTK12)
target_link_libraries(${PROJECT_NAME} PRIVATE Microsoft::DirectXShaderCompiler)
target_link_libraries(${PROJECT_NAME} PRIVATE glm::glm-header-only)
target_link_libraries(${PROJECT_NAME} PRIVATE cpptrace::cpptrace)
target_link_libraries(${PROJECT_NAME} PRIVATE unordered_dense::unordered_dense)

target_link_libraries(${PROJECT_NAME} PRIVATE nethost.lib)
configure_file(${dotnet-host_SOURCE_DIR}/runtimes/win-x64/native/nethost.dll ${CMAKE_BINARY_DIR}/bin/nethost.dll COPYONLY)

if (TARGET Microsoft::DirectX12-Agility)
    file(MAKE_DIRECTORY "${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/D3D12")
    add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
            COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_PROPERTY:Microsoft::DirectX12-Core,IMPORTED_LOCATION_RELEASE> $<TARGET_FILE_DIR:${PROJECT_NAME}>/D3D12
            COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_PROPERTY:Microsoft::DirectX12-Layers,IMPORTED_LOCATION_DEBUG> $<TARGET_FILE_DIR:${PROJECT_NAME}>/D3D12
            COMMAND_EXPAND_LISTS
    )
endif ()

set(SHADER_RE_TOOL_PATH "${shader-re-tool_SOURCE_DIR}/Coplt.ShaderReflections.Dxc.exe")

add_custom_target(shaders
        DEPENDS ${SHADERS_FILES}
        COMMAND ${deno_SOURCE_DIR}/deno.exe run -A ./scripts/build-shader.ts "${CMAKE_BINARY_DIR}" "${SHADERS_PATH}" "$<IF:$<CONFIG:DEBUG>,D,R>" "${DIRECTX_DXC_TOOL}" "${SHADER_RE_TOOL_PATH}"
        WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}
        VERBATIM
)

add_custom_target(dotnet
        COMMAND ${deno_SOURCE_DIR}/deno.exe run -A ./scripts/build-dotnet.ts "${CMAKE_BINARY_DIR}" "${CMAKE_CURRENT_SOURCE_DIR}/Game/Game.csproj" "$<IF:$<CONFIG:DEBUG>,D,R>"
        WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}
        VERBATIM
)

add_dependencies(${PROJECT_NAME} shaders)
add_dependencies(${PROJECT_NAME} dotnet)

get_cmake_property(_variableNames VARIABLES)
list(SORT _variableNames)
foreach (_variableName ${_variableNames})
    message(STATUS "${_variableName}=${${_variableName}}")
endforeach ()
