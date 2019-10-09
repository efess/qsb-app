
FROM mono:3.10-onbuild AS build
COPY Libraries/* ./
CMD [ "mono",  "./QSBApp.exe" ]
