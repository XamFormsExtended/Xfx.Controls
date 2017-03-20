var target = Argument("target", "Default");
var appName = "Xfx.Controls";
var configuration = Argument("configuration", "Release");

var touchDir = MakeAbsolute(Directory("./build-artifacts/output/touch"));
var droidDir=MakeAbsolute(Directory("./build-artifacts/output/droid"));


Setup(context =>
{
    var binsToClean = GetDirectories("./src/**/bin/");
	var artifactsToClean = new []{
        touchDir.ToString(), 
        droidDir.ToString()
	};
	CleanDirectories(binsToClean);
	CleanDirectories(artifactsToClean);

    //Executed BEFORE the first task.
    //  Information("Building version {0} of {1}.", semVersion, appName);
    //  Information("Tools dir: {0}.", EnvironmentVariable("CAKE_PATHS_TOOLS"));
});

Task("Default")
  .IsDependentOn("Pack");

Task("BuildDroid")
  .Does(() =>
{
  MSBuild("./src/Xfx.Controls.Droid/Xfx.Controls.Droid.csproj", new MSBuildSettings()
{Verbosity = Verbosity.Minimal}
      .WithProperty("OutDir", droidDir.ToString())
      .SetConfiguration(configuration));
});

Task("BuildTouch")
  .Does(() =>
{
  MSBuild("./src/Xfx.Controls.iOS/Xfx.Controls.iOS.csproj", new MSBuildSettings()
{Verbosity = Verbosity.Minimal}
      .WithProperty("OutDir", touchDir.ToString())
      .SetConfiguration(configuration));
});

Task("Pack")
  .IsDependentOn("BuildDroid")
  .IsDependentOn("BuildTouch")
  .Does(() => {
    var nuGetPackSettings   = new NuGetPackSettings {
                                    Id                      = appName,
                                    Version                 = "1.0.1.4",
                                    Title                   = "Xamarin Forms Extended Controls",
                                    Authors                 = new[] {"Chase Florell"},
                                    Description             = "Xamarin Forms Extended Controls",
                                    ProjectUrl              = new Uri("https://github.com/XamFormsExtended/Xfx.Controls"),
                                    Files                   = new [] {
                                                                        new NuSpecContent {Source = droidDir.ToString() + "/Xfx.Controls.dll", Target = "lib/portable-net45+win8+wpa81"},

                                                                        new NuSpecContent {Source = droidDir.ToString() + "/Xfx.Controls.Droid.dll", Target = "lib/MonoAndroid"},
                                                                        new NuSpecContent {Source = droidDir.ToString() + "/Xfx.Controls.dll", Target = "lib/MonoAndroid"},

                                                                        new NuSpecContent {Source = touchDir.ToString() + "/Xfx.Controls.iOS.dll", Target = "lib/Xamarin.iOS10"},
                                                                        new NuSpecContent {Source = touchDir.ToString() + "/Xfx.Controls.dll", Target = "lib/Xamarin.iOS10"},
                                                                      },
                                    Dependencies            = new [] { new NuSpecDependency{
																		Id       = "Xamarin.Forms",
																		Version  = "2.0"
																	}},
                                    BasePath                = "./src",
                                    NoPackageAnalysis       = true,
                                    OutputDirectory         = "./.nuget"
                                };

    NuGetPack(nuGetPackSettings);
  });

RunTarget(target);