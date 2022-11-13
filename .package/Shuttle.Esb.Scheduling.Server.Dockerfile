FROM mcr.microsoft.com/dotnet/runtime:6.0

COPY ./bin/. /opt/shuttle.esb.scheduling.server/.

RUN chmod +x /opt/shuttle.esb.scheduling.server/Shuttle.Esb.Scheduling.Server

ENTRYPOINT ["/opt/shuttle.esb.scheduling.server/Shuttle.Esb.Scheduling.Server"]