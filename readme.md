# What we developed:
- Core Blazor App with Background Services
- Subscribe to MQTT Broker via MQTT.NET
- Dashboard HMI style with alarms, gauges, kpi
- data buffering in csv file/per minute
- data in parquet for every csv file

hot=immediate
warm=buffering 1m

- json status and alarm (hot) send to iot hub
- (evt) json status and alarm (hot) send to event grid
- parquet (warm) send to iot hub
- parquet send to data lake
- parquet send to onelake
- (evt) json to rti

# Useful links:
- https://blazor.radzen.com/dashboard?theme=fluent
- https://fonts.google.com/icons?icon.set=Material+Symbols&icon.size=24&icon.color=%23e8eaed
