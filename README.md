# AuroraPatch

This allows patching of the Aurora executable, it supports Harmony. The patcher works with any Aurora version, it is up to each individual patch author to keep their patches working with new Aurora executables.

## For users

AuroraPatch should be installed in Aurora's folder. Patches go into their own `\Patches\{name}` subfolder where `name` is the name of the patch.

## For patch creators

Your patch should be a Class Library targeting the .Net 4 Framework (same as Aurora itself). You create a patch by extending the `AuroraPatch.Patch` class. See the ExamplePatch project.
