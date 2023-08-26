# iplust
一个用于生成`图在上面文字在下面的弔图`(抽象描述)的工具

# 安装方法
本工具已上传至[NuGet](https://www.nuget.org/packages/iplust/)
## 通过dotnet tool（推荐）
1. 下载 .net6 sdk 并安装
2. 在命令行中运行 `dotnet tool install iplust --global`
3. 在命令行中运行 `iplust`

## 通过Release
1. 下载 .NET运行时 并安装
2. 在 Release 页面下载iplust并解压
3. 使用命令行进入解压目录后运行 `iplust.exe`

# 使用方法
目前命令行参数支持情况
```
Description:
  把文字放在图片下面组成一个表情包

Usage:
  iplust <image> <text> [options]

Arguments:
  <image>  你要加字的图片
  <text>   你要加的字

Options:
  --font <font>   字体
  --reserve       用白色的字和黑色的底
  --debug         Debug mode
  -o <o>          输出文件名
  --outdated      遗照
  --version       Show version information
  -?, -h, --help  Show help and usage information
```

## 举例
一张图片存在于 `D:/EmptyFolder/qwq.jpg`  
使用PS打开后目录位于 `D:/`  
使用命令(这里为通过dotnet tool的方式调用) `iplust EmptyFolder/qwq.jpg qmq`  
命令执行完成后图片将会被保存在 `D:/qwq_qmq.jpg`  
更多参数用途自行发掘  

## 关于自定义字体
将字体导入系统或放入`<程序目录>/Fonts`内  
再使用 `--font <FontFamilyName>` 以指定生成字体
若不使用 `--font` 参数则默认使用 `Microsoft YaHei`

# 欢迎贡献
