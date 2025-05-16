import type { NextConfig } from 'next';

const nextConfig: NextConfig = {
   reactStrictMode: true,
   images: {
      // This isn't advised. Need a "media" server to pass the images through.
      remotePatterns: [
         {
            protocol: 'https',
            hostname: '**',
         },
         {
            protocol: 'http',
            hostname: '**',
         },
      ],
   },
};

export default nextConfig;
