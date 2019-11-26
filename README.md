Simple.Wpf.Composition
======================

[![Build status](https://ci.appveyor.com/api/projects/status/o4xdboa6ec5nu8g2/branch/master?svg=true)](https://ci.appveyor.com/project/oriches/simple-wpf-composition/branch/master)

If you're reading this you're probably wondering what this git repo is about...

Simply this is example code I'ved used to do composition in WPF, it's all MVVM and uses an IoC container through out. I use the IoC container to created nested (child) lifetime scopes for the main ViewModels, these are created by a WorkspaceDescriptor, contained inside a Workspace and managed by the application chrome (infrastructure), when a Workspace is closed the Workspace is disposed and the nested lifetime scope in the IoC container is disposed - everything is cleaned up! The following blog post explains more - http://awkwardcoder.blogspot.co.uk/2013/11/using-ioc-nested-lifetime-scopes-with.html

*Why I did this?*

I see PRISM used a lot in the wrong places, it's a sledgehammer to crack a nut! This results in slow application startup - the splash screen showing the application modules being loaded & resolved, users don't want to see this, they want to use your application to get on with their jobs.

  1. If you don't have true dynamic composition at runtime why use modules etc,
  2. Why use MEF at all when your application is not dynamic, an IoC container does everything you need,
  3. Why do layout composition in anything but XAML, it's what it's designed for!
  

A big shout out to [David Hanson](https://github.com/Davidhanson90) for the inspiration - to keep him happy!

 
  
 
  

