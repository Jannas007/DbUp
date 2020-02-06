# DbUp
Contains a sample for the Octopus Deploy blog post on creating a DBUp application


# Execution examples:


``` To create a DB from scratch:

~/Documents/VisualStudioCode/publish/DbUp --ConnectionString="Server=127.0.0.1;Port=3306; Database=test; Uid=root; Pwd=xxxxxx;" --DBCreateScripts="/home/jannas/dvc/ldp_db_scripts/CreateBaseDBScripts"

### On Windows:

DbUp.exe --ConnectionString="Server=127.0.0.1;Port=3306; Database=test; Uid=root; Pwd=xxxxx;" --DBCreateScripts="z:\dvc\ldp_db_scripts\CreateBaseDBScripts"

```

``` to run Update scripts:

~/Documents/VisualStudioCode/publish/DbUp --ConnectionString="Server=127.0.0.1;Port=3306; Database=Configuration; Uid=root; Pwd=xxxxx;" --UpdateScripts="/home/jannas/dvc/ldp_db_scripts/Configuration/UpdateScripts"

### On Windows:

DbUp.exe --ConnectionString="Server=127.0.0.1;Port=3306; Database=Configuration; Uid=root; Pwd=xxxx;" --UpdateScripts="z:\dvc\ldp_db_scripts\Configuration\UpdateScripts"

```



