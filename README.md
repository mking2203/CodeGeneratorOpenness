# CodeGeneratorOpenness
Siemens TIA Portal Code Generator via Openness Interface

Since we are doing import of Graph step sequence through an Excel sheet using Openness, I was thinking of building a "structure" code generator for the TIA portal. Right now, most important functions are working so it would be possible to
<br>
-build a group tree<br>
-import any kind of blocks like FB, FC, OB or data types<br>
-generate step sequences as graph (not 100% complete yet) - no config right now

This covers mainly the function we can find in the Openness scripter. But I want to go a little further to build structures more variable. Not sure where it leads to but my goal will be to use templates to build FB with n-times valve FBs through a scripter or later on maybe a tool to configure this.

Based on the description found here:

https://support.industry.siemens.com/cs/attachments/109477163/TIAPortalOpennessenUS_en-US.pdf (English)
https://cache.industry.siemens.com/dl/files/163/109477163/att_926040/v1/TIAPortalOpennessdeDE_de-DE.pdf (Deutsch)


This version is based on TIA V16.
<br>
With some small changes this will also work with 14SP1,15 or 15.1 (work ongoing)
<br>
<br>
Functions added:<br>
<br>
-open TIA with interface (first instance or new instance)<br>
-open project file via file dialog<br>
-compile software<br>
-save project<br>
-close project<br>
-show folder structure software<br>
-show data types<br>
-add / delete groups in the treeview<br>
-add complete path to the treeview<br>
-imports PLC blocks (with rename if needed)<br>
-import data types<br>
-export blocks / types<br>
-export project text (de/en)<br>
-import project text (de/en)<br>
<br>
Test<br>
-add / change language for editing<br>
-for devolpment set the key in the registry to avoid firewall each time<br>
<br>
Limtitations:<br>
-no global search for block/types<br>
-import fails if language is not in the project<br>
-export projects text can not overwrite existing files
<br>
<br>
Screenshot:
<br>
<img src="https://raw.githubusercontent.com/mking2203/CodeGeneratorOpenness/master/CodeGenerator.png" alt="Code Generator">

<br>
<br>
Some code for the graph generation is "reversed engineered" since there is no description.
<br>
For the sequence generation I used a V14 sample of an empty seqeunce. In the XML we need to add transitions and steps into the static area (change in later versions) Then I enumerate through my defined steps to build the sequence including transitions and branches. Of course I can not cover 100% of any posibility, so my sequences are straight forward - only with alternate branches and jumps. In the next version I will also cover actions and supervisions for each step.
<br>
<img src="https://github.com/mking2203/CodeGeneratorOpenness/raw/master/Sample/Sequence.png" alt="Step sequence">

