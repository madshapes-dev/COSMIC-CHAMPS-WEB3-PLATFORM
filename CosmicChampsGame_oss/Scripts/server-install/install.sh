#!/bin/bash

sudo dnf install -y nginx
sudo yum install -y gettext

export PUBLIC_HOSTNAME=`curl http://URL/latest/meta-data/public-hostname`
COSMIC_WEBGLPROXY_CONF=`envsubst '${PUBLIC_HOSTNAME}' < nginx/conf.d/cosmic-webglproxy.template`
INSTANCE_ID=`curl http://URL/latest/meta-data/instance-id`

echo "{ \"instanceId\": \"$INSTANCE_ID\" }" > instance-metadata.json
echo "$COSMIC_WEBGLPROXY_CONF" | sudo tee nginx/conf.d/cosmic-webglproxy.conf > /dev/null
sudo cp -r nginx/* /etc/nginx/
at -f run-nginx.sh now + 10 min