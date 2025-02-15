# Copied from https://github.com/dotnet/dotnet-docker/blob/08e7f8d581f4328aeb3585233aa14017dc2bee30/src/sdk/8.0/bookworm-slim/amd64/Dockerfile
FROM mcr.microsoft.com/devcontainers/dotnet:1-6.0

ENV \
    # Do not generate certificate
    DOTNET_GENERATE_ASPNET_CERTIFICATE=false \
    # Do not show first run text
    DOTNET_NOLOGO=true \
    # Enable correct mode for dotnet watch (only mode supported in a container)
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    # Skip extraction of XML docs - generally not useful within an image/container - helps performance
    NUGET_XMLDOC_MODE=skip

#hacks to support netcore3.1.x
RUN filename=libicu63_63.1-6+deb10u3_amd64.deb \
    && curl -fSLO http://ftp.us.debian.org/debian/pool/main/i/icu/$filename \
    && apt-get install -y ./$filename \
    && rm $filename

RUN filename=libssl1.1_1.1.1n-0+deb10u6_amd64.deb \
    && curl -fSLO http://security.debian.org/debian-security/pool/updates/main/o/openssl/$filename \
    && apt-get install -y ./$filename \
    && rm $filename

RUN dotnet_version=3.1.426 \
    && curl -fSL --output dotnet.tar.gz https://builds.dotnet.microsoft.com/dotnet/Sdk/$dotnet_version/dotnet-sdk-$dotnet_version-linux-x64.tar.gz \
    && dotnet_sha512='6c3f9541557feb5d5b93f5c10b28264878948e8540f2b8bb7fb966c32bd38191e6b310dcb5f87a4a8f7c67a7046fa932cde3cce9dc8341c1365ae6c9fcc481ec' \
    && echo "$dotnet_sha512  dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -oxzf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz

RUN dotnet_version=8.0.406 \
    && curl -fSL --output dotnet.tar.gz https://builds.dotnet.microsoft.com/dotnet/Sdk/$dotnet_version/dotnet-sdk-$dotnet_version-linux-x64.tar.gz \
    && dotnet_sha512='d6fdcfebd0df46959f7857cfb3beac7de6c8843515ece28b24802765fd9cfb6c7e9701b320134cb4907322937ab89cae914ddc21bf48b9b6313e9a9af5c1f24a' \
    && echo "$dotnet_sha512  dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -oxzf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz \
    # Trigger first run experience by running arbitrary cmd
    && dotnet help
