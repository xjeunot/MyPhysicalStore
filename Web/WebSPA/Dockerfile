FROM node AS builder
#Download Package Enterprise CA Security : Start Section
WORKDIR /usr/local/share/ca-certificates
COPY Security/cert_rootcagfi.crt cert_rootcagfi.crt
RUN update-ca-certificates
#Download Package Enterprise CA Security : End Section
WORKDIR /app
COPY . .
WORKDIR "/app/Web/WebSPA"
RUN npm install -g @angular/cli
RUN npm ci
RUN ng build --prod

FROM nginx:alpine
EXPOSE 80
COPY --from=builder /app/Web/WebSPA/dist/WebSPA/ /usr/share/nginx/html/
