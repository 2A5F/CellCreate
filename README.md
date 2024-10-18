# CellCreate

A factory game

## Build 构建

- Used gitmodule  
  使用了 git 子模块
- Env req 环境需求
  - dx12.2 gpu
  - windows 11
  - pwsh 7
  - cmake, ninja, clang (c++ 23, clang-cl)
  - vcpkg
  - .net 8 sdk
  - [ClangSharpPInvokeGenerator](https://github.com/dotnet/ClangSharp)
    ```
    dotnet tool install --global ClangSharpPInvokeGenerator --version <VERSION>
    ```
