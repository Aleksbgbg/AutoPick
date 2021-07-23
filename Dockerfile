FROM ubuntu:latest

COPY AutoPick.Server autopick-server
COPY docker-setup.sh docker-setup.sh
COPY docker-start.sh docker-start.sh
RUN /docker-setup.sh

WORKDIR /server
ENTRYPOINT /bin/sh /docker-start.sh && /bin/sh
