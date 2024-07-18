﻿using Soario.Utils;
using Soario.Windowing;

namespace Soario;

public static class App
{
    public static Window MainWindow = null!;
    public static void Start()
    {
        Console.WriteLine("Start");
        MainWindow = new Window(new() { Title = "测试", Size = new(1280, 720), MinSize = new(640, 360) });
    }
}
