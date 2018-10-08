#父镜像
FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base

#设置工作目录
WORKDIR /app

#复制发布文件到/app下
COPY . /app

#设置端口
EXPOSE 80

#使用dotnet LisAPI.dll来运行asp.net core项目，注意大小写
ENTRYPOINT ["dotnet", "InvoicingService.dll"]
