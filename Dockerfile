# FROM mono:3.10-onbuild AS build

# COPY Libraries/* ./

# CMD [ "mono",  "./QSBApp.exe" ]

FROM mono:6.12.0.107 

RUN mkdir -p /usr/bin/app

WORKDIR /usr/bin/app

COPY QSB.App/bin/Debug/* ./
COPY QSB.Data/bin/Debug/* ./

CMD [ "mono",  "./QSBApp.exe" ]