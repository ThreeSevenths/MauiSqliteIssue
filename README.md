# Maui Issue Repo

This code has an issue for me in Release mode when deployed in the emulator or on a local device. Around the 1000 mark, the app crashes consistently. 

The file errorlog.txt shows my adb logcat at the crash time. 

The relevant first signal seems to be

```txt
mauiperftesting: * Assertion at /__w/1/s/src/mono/mono/mini/aot-runtime.c:5244, condition `plt_entry' not met
```

This app is basically the out of the box starter with a method call to run inserts on a sqlite database instead of the counter button.

The issue does not seem to happen in debug builds. It only seems to manifest in release builds.
