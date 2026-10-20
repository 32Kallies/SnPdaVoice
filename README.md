# Subnautica PDA Voice Generator

## Online version

For the online version of this, visit <https://subnauticapdavoice.com/>

## Self Hosted

### Build instructions

#### Windows

1. Download, install and run Docker Desktop
2. Install dotnet 9 for your machine (probably x64 Windows) <https://dotnet.microsoft.com/en-us/download/dotnet/9.0>
3. Clone this repository to somewhere in Windows (or download it as a zip and extract it), note down the path for later steps, e.g. `C:\Users\MYUSERNAME\Downloads\SnPdaVoice-main`
4. Open powershell and change directory to inside this cloned repository, e.g. `cd C:\Users\MYUSERNAME\Downloads\SnPdaVoice-main`
5. Build the image, `docker build -t pda-voice-generator .`
6. Change directory to the `VoiceGenAPI` directory inside this cloned repository, e.g. `cd C:\Users\MYUSERNAME\Downloads\SnPdaVoice-main\VoiceGenAPI`
7. Run the API and leave it running in this powershell window (it should say "Waiting for a new client"), `dotnet run --project .\VoiceProcessorServer\VoiceProcessorServer.csproj`
8. Open a second powershell in Windows and run the container we built before, e.g. `docker run --rm -it -p 8005:3000 -v "C:\Users\MYUSERNAME\Downloads\SnPdaVoice-main\VoiceGenAPI\voice-processing-container\temp:/app/output" pda-voice-generator:latest`
9. Open your browser and navigate to <http://127.0.0.1:8005>

If you get an error about, "No matching voice installed" when trying to create your first sound:

1. Edit the file, `C:\Users\MYUSERNAME\Downloads\SnPdaVoice-main\VoiceGenAPI\voice-processing-container\voices\PDA.json`
2. Look for the line `"voice_name": "IVONA 2 Amy OEM"` and change it to `"voice_name": "Microsoft Hazel Desktop"` which is a newer voice bundled with Windows.

You can check which voice synthesisers you have installed in Windows with these commands in powershell, in case Hazel doesn't work:

```ps
Add-Type -AssemblyName System.Speech
$s = New-Object System.Speech.Synthesis.SpeechSynthesizer
$s.GetInstalledVoices() | ForEach-Object { $_.VoiceInfo.Name }
```
