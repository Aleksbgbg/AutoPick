# Install .NET
apt update && \
apt install wget -y && \

wget https://packages.microsoft.com/config/ubuntu/21.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
dpkg -i packages-microsoft-prod.deb && \
rm packages-microsoft-prod.deb && \

apt update && \
apt install -y apt-transport-https dotnet-sdk-5.0 aspnetcore-runtime-5.0

# Build server
mkdir server
(cd autopick-server && dotnet publish -c Release)
cp -r autopick-server/bin/Release/net5.0/publish/* server
