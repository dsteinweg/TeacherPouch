dotnet publish -c Release -o .\bin\publish -r linux-x64 --self-contained

docker build `
  -f .\bin\publish\Dockerfile `
  -t dsteinweg/teacherpouch `
  .\bin\publish

docker push dsteinweg/teacherpouch:latest
