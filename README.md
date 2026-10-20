# https://subnauticapdavoice.com/

# Build instructions

## Windows:

> [!WARNING]
> As of October 2026, I replaced the entire backend with a far more accurate solution, but I cannot provide all the assets needed to run the program. It now uses FL Studio to process the filters. If you must build it and do not own FL Studio, you may have more luck on the `legacy` branch of this repository.

1. Obtain the IVONA Amy TTS voice (ideally the OEM version) and install it. Unfortunately, the best version does not appear to be free.
2. Download and install WLS (Ubuntu 24.04) and Docker Desktop
3. Open the folder in the command terminal and run `docker build -t pda-voice-generator .`
4. Once it succesfully is installed, run the image in Docker Desktop
5. Open the correct URL in your browser
