FROM alpine:latest AS build
WORKDIR /app
COPY . .

ENV PNPM_HOME=/root/.local/share/pnpm
ENV PATH=$PNPM_HOME:$PATH

RUN apk add --no-cache curl wget bash
RUN wget -qO- https://get.pnpm.io/install.sh | ENV="$HOME/.shrc" SHELL="$(which sh)" sh -
RUN pnpm i

FROM caddy:alpine
COPY --from=build /app /usr/share/caddy
