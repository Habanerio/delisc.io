'use client';

import React, { useEffect, useState } from 'react';
import Image from 'next/image';
import Link from 'next/link';

import { LinkItem } from '@//models';
import { TagBadges } from '../tags';

const brokenImg = '/img/no-image-found.png';

export default function LinkCard(item: LinkItem) {
   const [src, setSrc] = useState('/img/blank-image.png');

   let imgUrl =
      item.imageUrl && item.imageUrl?.startsWith('//')
         ? `https:${item.imageUrl}`
         : item.imageUrl;

   imgUrl =
      item.imageUrl &&
      item.imageUrl.length > 0 &&
      item.imageUrl.startsWith('http')
         ? item.imageUrl
         : item.url && item.url.indexOf('duckduckgo') >= 0
           ? '/img/duckduckgo-logo.jpg'
           : item.url && item.url.indexOf('google') >= 0
             ? '/img/google-logo.jpg'
             : item.url && item.url.indexOf('reddit') >= 0
               ? '/img/reddit-logo.png'
               : '/img/no-image-found.png';

   useEffect(() => {
      setSrc(imgUrl);
   }, [item]);

   const title = item.title || '';

   const description = item.description
      ? item.description.length > 100
         ? item.description.slice(0, 97) + '...'
         : item.description
      : '&nbsp;';

   var tagBadges =
      item.tags && item.tags.length > 0 ? <TagBadges tags={item.tags} /> : null;

   /**
    * Generate the footer with tags.
    */
   const getTagFooter = tagBadges ? (
      <div className='p-2 bg-gray-200 rounded-md overflow-hidden h-[5.2rem]'>
         {tagBadges}
      </div>
   ) : null;

   const getFooter = () => {
      return <div className='p-2 bg-gray-200 rounded-md'>{tagBadges}</div>;
   };

   /**
    * If there's an error when loading the image, set the src to a `broken image`
    * @param e
    */
   const onImgError = (e: any) => {
      console.log(e);
      setSrc(brokenImg);
   };

   return (
      <div
         key={item.id}
         className='link-card flex flex-col border-1 border-black rounded-md shadow-sm bg-white mb-4'>
         <Link
            href={`/link/${item.id}`}
            key={item.id}
            className='flex flex-1 flex-col'>
            <Image
               src={src}
               alt={title}
               dataset-src={src}
               width={0}
               height={0}
               sizes='100vw'
               className='object-cover rounded-t-md'
               style={{ width: '100%' }}
               //    priority={priority}
               onError={(e) => onImgError(e)}
            />
            <div className='p-2'>
               <h5 className='text-gray-900 font-bold truncate'>{title}</h5>
               <p>
                  via : <span>{item.domain}</span>
               </p>
               <p
                  className='flex flex-1'
                  dangerouslySetInnerHTML={{ __html: description }}></p>
            </div>
         </Link>
         {getTagFooter}
      </div>
   );
}
