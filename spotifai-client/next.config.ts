import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  /* config options here */
  NEXT_PUBLIC_BACKEND_ROOT_URL: process.env.NEXT_PUBLIC_BACKEND_ROOT_URL,
  async headers() {
    return [
      {
        // Routes this applies to
        source: "/",
        // Headers
        headers: [
          {
            key: "Access-Control-Allow-Origin",
            value: "*"
          },

          {
            key: "Access-Control-Allow-Methods",
            value: "GET, POST, PUT, DELETE, OPTIONS"
          },

          {
            key: "Access-Control-Allow-Headers",
            value: "Content-Type, Authorization"
          }
        ]
      }
    ];
  }
};

export default nextConfig;
