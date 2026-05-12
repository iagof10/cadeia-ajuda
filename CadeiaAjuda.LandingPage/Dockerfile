FROM nginx:alpine
WORKDIR /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY index.html .
COPY styles.css .
COPY main.js .
COPY image ./image

EXPOSE 8080

CMD ["nginx", "-g", "daemon off;"]
