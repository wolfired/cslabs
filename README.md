# cslabs

## usage

```bat
rem 重置项目
clean

rem 重建项目
build

rem 重建并运行指定程序
run testbed

run plantor -h
run plantor -o E:\Desktop\uml\test.txt -i E:\workspace_git\cslabs\bin\planter.dll
run plantor -o E:\Desktop\uml\test.txt -i D:\2019.4.0f1\Editor\Data\Managed\UnityEngine\UnityEngine.dll 
run plantor -o E:\Desktop\uml\test.txt -i D:\2019.4.0f1\Editor\Data\Managed\UnityEngine\UnityEngine.AudioModule.dll
run plantor -o E:\Desktop\uml\test.txt -i D:\2019.4.0f1\Editor\Data\Managed\UnityEngine\UnityEngine.CoreModule.dll -t UnityEngine.Camera
run plantor -o E:\Desktop\uml\test.txt -i D:\2019.4.0f1\Editor\Data\Managed\UnityEngine\UnityEngine.CoreModule.dll -t UnityEngine.Behaviour
```

## projects

### testbed

应用程序, 依赖 `tools` 和 `utils`

### svn_exporter

应用程序, `svn export` 指定后缀文件

```bat
svn_exporter -i .png -i .jpg -u url/to/be/exported -o export/to/path
```

### tools

C库

### utils

C#库

### plantor

应用程序, 依赖 `planter`

### planter

C#库
