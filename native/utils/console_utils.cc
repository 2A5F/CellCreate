#include "console_utils.h"

#ifdef _WINDOWS
#include <windows.h>
#include <cstdio>
#include <fcntl.h>
#include <io.h>
#include <iostream>
#include <fstream>
#endif

void RedirectIOToConsole()
{
#ifdef _WINDOWS
    if (AttachConsole(ATTACH_PARENT_PROCESS) == false) return;

    HANDLE ConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);
    const int SystemOutput = _open_osfhandle(reinterpret_cast<intptr_t>(ConsoleOutput), _O_TEXT);

    // check if output is a console and not redirected to a file
    if (_isatty(SystemOutput) == false) return; // return if it's not a TTY

    FILE* COutputHandle = _fdopen(SystemOutput, "w");

    // Get STDERR handle
    HANDLE ConsoleError = GetStdHandle(STD_ERROR_HANDLE);
    const int SystemError = _open_osfhandle(reinterpret_cast<intptr_t>(ConsoleError), _O_TEXT);
    FILE* CErrorHandle = _fdopen(SystemError, "w");

    // Get STDIN handle
    HANDLE ConsoleInput = GetStdHandle(STD_INPUT_HANDLE);
    const int SystemInput = _open_osfhandle(reinterpret_cast<intptr_t>(ConsoleInput), _O_TEXT);
    FILE* CInputHandle = _fdopen(SystemInput, "r");

    //make cout, wcout, cin, wcin, wcerr, cerr, wclog and clog point to console as well
    std::ios::sync_with_stdio(true);

    // Redirect the CRT standard input, output, and error handles to the console
    freopen_s(&CInputHandle, "CONIN$", "r", stdin);
    freopen_s(&COutputHandle, "CONOUT$", "w", stdout);
    freopen_s(&CErrorHandle, "CONOUT$", "w", stderr);

    //Clear the error state for each of the C++ standard stream objects.
    std::wcout.clear();
    std::cout.clear();
    std::wcerr.clear();
    std::cerr.clear();
    std::wcin.clear();
    std::cin.clear();
#endif
}
