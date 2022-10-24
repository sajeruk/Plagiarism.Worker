Plagiarism.Worker
==

**Note**: this repository has been archived. The reasons are as follows: first, the patches
made here compared to the upstream are just not needed now, and, second, iRunner2 now has a
better plagarism detection system and doesn't use this worker anymore.

Plagiarism module for iRunner2. Windows service, working with iRunner2 via API

Install the service
-------------------
```
> cd C:\Plagiarism.Worker
> C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe Plagiarism.Worker.exe
```
Then open `services.msc` and launch the newly created service.
