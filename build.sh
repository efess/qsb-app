aws ecr-public get-login-password --region us-east-1 | docker login -u AWS --password-stdin "https://public.ecr.aws"
docker build -t public.ecr.aws/v9s4j9b0/quake-stats:latest .
docker push public.ecr.aws/v9s4j9b0/quake-stats:latest