Imagem base leve para aplica��es autocontidas
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0 AS runtime
WORKDIR /app
COPY ./publish .
RUN chmod +x ./PatsyLibrary
ENTRYPOINT ["./PatsyLibrary"]