donet publish -c Release

docker build \
  -f ./Dockerfile \
  -t registry.dsteinweg.com/teacherpouch \
  bin/Release/netcoreapp1.1/publish
