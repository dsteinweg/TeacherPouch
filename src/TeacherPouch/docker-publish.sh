dotnet publish -c Release

docker build \
  -f ./bin/Release/net7.0/publish/Dockerfile \
  -t registry.dsteinweg.com/teacherpouch \
  bin/Release/net7.0/publish
