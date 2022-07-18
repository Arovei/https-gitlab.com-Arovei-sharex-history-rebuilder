Simple console app which will construct a History.json file in the base directory of any drive it has write permissions (default is D:).
The program's main task is to just add the files so they can be indexed by ShareX's History search.

Small issues:
The JSON file has one pair of extra brackets (at the start and end) which I am unsure of if they cause an error if you just use that file, as I don't know if ShareX can append to it without issue. If you already have an existing History.json I suggest backing it up before adding to it.

Due to naming schemes being custom I didn't put much effort into getting the ProcessName, so if it can't figure it out it just gives it Unknown or a partial name.
