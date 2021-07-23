docker build -t auto-pick-server .
docker run --name auto-pick-server -p 0.0.0.0:80:8080 -id auto-pick-server
