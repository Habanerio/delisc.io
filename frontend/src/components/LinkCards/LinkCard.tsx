'use client';

import React, { useEffect, useState } from 'react';
import Image from 'next/image';
import Link from 'next/link';

import { LinkItem } from '@//models';
import { TagBadges } from '../tags';

const brokenImg = '/no-image-found.png';

export default function LinkCard(item: LinkItem) {
   let imgUrl =
      item.imageUrl && item.imageUrl?.startsWith('//') ? `https:${item.imageUrl}` : item.imageUrl;

   imgUrl =
      item.imageUrl && item.imageUrl.length > 0 && item.imageUrl.startsWith('http')
         ? item.imageUrl
         : item.url && item.url.indexOf('duckduckgo') >= 0
           ? '/img/duckduckgo-logo.jpg'
           : item.url && item.url.indexOf('google') >= 0
             ? '/img/google-logo.jpg'
             : item.url && item.url.indexOf('reddit') >= 0
               ? '/img/reddit-logo.png'
               : '/img/no-image-found.png';

   const [src, setSrc] = useState('/img/blank-image.png');

   useEffect(() => {
      setSrc(imgUrl);
   }, [item]);

   const title = item.title; //&& item.title.length > 50 ? item.title.slice(0, 47) + '...' : item.title || '';

   const description =
      item.description && item.description.length > 100
         ? item.description.slice(0, 97) + '...'
         : item.description;

   var tagBadges = item.tags && item.tags.length > 0 ? <TagBadges tags={item.tags} /> : null;

   /**
    * Generate the footer
    */
   const tagFooter = tagBadges ? <div className='p-2 bg-gray-200'>{tagBadges}</div> : null;

   /**
    * If there's an error when loading the image, set the src to a `broken image`
    * @param e
    */
   const onImgError = (e: any) => {
      console.log(e);
      setSrc(brokenImg);
   };

   return (
      <div key={item.id} className='border-1 rounded-md shadow-sm bg-white'>
         <Link href={`/link/${item.id}`} key={item.id} className='flex flex-col h-full'>
            <Image
               src={src}
               alt={title}
               width={0}
               height={0}
               sizes='100vw'
               className='object-cover rounded-t-md'
               style={{ width: '100%', height: 'auto' }}
               //    priority={priority}
               onError={(e) => onImgError(e)}
            />
            <div className='flex flex-col flex-1 p-2'>
               <h5 className='blo text-gray-900 font-bold truncate'>{title}</h5>
               <p>
                  via : <span>{item.domain}</span>
               </p>
               <p className=''>{description}</p>
            </div>
            {tagFooter}
         </Link>
      </div>
   );
}
