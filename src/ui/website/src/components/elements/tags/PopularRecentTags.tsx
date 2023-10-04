'use client';

import React, { useEffect, useState } from 'react';
import { usePathname } from 'next/navigation';
import useSWR from 'swr';

import Link from 'next/link';

import styles from './Tags.module.scss';

import { TagResult } from '@/types/tags';

const colorOptions = [
   'bg-deliscio',
   'bg-primary',
   'bg-secondary',
   'bg-success',
   'bg-warning text-dark',
   'bg-info text-dark',
   //    'bg-light text-dark',
   'bg-deliscio text-dark',
   'bg-primary text-dark',
   'bg-secondary text-dark',
   'bg-success text-dark',
];

const TITLE_POPULAR = 'Popular Tags';
const TITLE_RELATED = 'Related Tags';

const PopularRecentTags = (props: PopularRecentTagsProps) => {
   const [data, setData] = useState<TagResult[] | null>(null);
   const [isLoading, setLoading] = useState<boolean>(true);
   const [error, setError] = useState<any>(null);

   // Get the pathName from the url
   const originalPathName = usePathname();

   const pathName = usePathname() === '/' ? '' : usePathname().replace('/links/tags/', '');
   const title = pathName?.replace('/', '').length > 0 ? TITLE_RELATED : TITLE_POPULAR;

   const tagPath = encodeURIComponent(pathName.replaceAll('+', ' ').replaceAll('/', ','));
   const count = props.count && props.count > 0 ? props.count : 25;

   // NOTE: This is called twice. Apparently it's because strict mode is true (next.config.js: reactStrictMode)
   useEffect(() => {
      console.log(`Use Effect: ${originalPathName}`);
      fetch(`/api/links/tags?tags=${tagPath}&count=${count}`, {
         headers: {
            'Content-Type': 'application/json',
         },
      })
         .then((res) => res.json())
         .then((data) => {
            //const d = data as TagResult[];
            setData(data);
            setLoading(false);
         })
         .catch((err) => {
            console.log(err);
            setError(err);
         });
   }, [title]);

   if (isLoading) return <p>Loading...</p>;
   if (!data) return <p>Failed to load Tags</p>;

   if (error) return <p>Failed to load Tags: {error}</p>;

   if (!data) {
      return <p>Loading Tags....</p>;
   }

   const maxWeight = Math.max(...data.map((tag) => tag.weight));

   const tagItems = data
      ? data.map((tag, idx) => {
           const tagSize =
              tag.weight / maxWeight >= 0.0001 ? (tag.weight / maxWeight) * 1 + 1 : 0.8;
           let colorOption =
              (idx <= colorOptions.length
                 ? colorOptions[idx]
                 : colorOptions[idx % colorOptions.length]) ?? 'bg-white text-dark';

           const href = `/links/tags/${pathName}/${tag.name.replaceAll(' ', '+')}`;
           return (
              <li
                 key={tag.name}
                 style={{ display: 'inline-block', marginRight: '10px', marginBottom: '10px' }}>
                 <Link
                    href={href}
                    className={colorOption}
                    data-count={tag.count}
                    data-weight={tag.weight}
                    data-percent={tag.weight / maxWeight}
                    style={{
                       fontSize: `${tagSize}rem`,
                       padding: '5px',
                       borderRadius: '0.5rem',
                       wordBreak: 'break-word',
                    }}>
                    {tag.name} <span>[{tag.count}]</span>
                 </Link>
                 &nbsp;
              </li>
           );
        })
      : null;

   return (
      <div className={`card ${styles.card}`} style={{ width: '100%' }}>
         <div className={`card-header ${styles['card-header']}`}>{title}</div>
         <div className='card-body'>
            <ul className='list-unstyled'>{tagItems}</ul>
         </div>
      </div>
   );
};

/**
 * PopularRecentTagsProps
 * @typedef PopularRecentTagsProps
 * @property {string} baseApi: The api's base url so that we can retrieve the tags.
 * At the moment it's needed as client-side can't read .env files
 * @property {number} [count=25]: The number of tags to retrieve
 */
type PopularRecentTagsProps = {
   count?: number | 25;
};

export default PopularRecentTags;
