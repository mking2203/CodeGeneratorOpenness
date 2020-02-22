# CodeGeneratorOpenness
Siemens TIA Portal Code Generator via Openness Interface

Based on the description found here:

https://cache.industry.siemens.com/dl/files/163/109477163/att_926040/v1/TIAPortalOpennessdeDE_de-DE.pdf


This version is based on TIA V16. You need to reference the DLLs from

C:\Program Files\Siemens\Automation\Portal V16\PublicAPI\V16

With some small changes this will also work with 14SP1,15 or 15.1

Functions added:<br>

-open TIA with interface (first instance or new instance)<br>
-open project file via file dialog<br>
-close project<br>
-add / change language for editing<br>
-import a FC<br>





Some code for the Graph generation is "reversed engineered" since there is no description.

