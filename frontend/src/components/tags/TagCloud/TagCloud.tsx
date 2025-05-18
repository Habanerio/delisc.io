'use client';

import { useEffect, useState } from 'react';

import { useFetchTags } from '@/hooks/apis/links/useGetTags';
import { LinkTag } from '@/models/LinkTag';
import { TagBadges } from '../TagBadges';

/**
 * TagCloud
 * @typedef TagCloud
 * @property {number} [maxTags=50]: The number of tags to retrieve
 * @property {string[]} [currentTags]: The current tags, prior to the search
 */
type Props = {
   maxTags?: number; // Removed default here, will be handled in fetchTags or useEffect
   currentTags?: string[];
};

export function TagCloud({ maxTags, currentTags }: Props) {
   const [existingTags, setExistingTags] = useState<string[] | null>(null);
   const [newTags, setNewTags] = useState<LinkTag[]>([]);
   const [isLoading, setLoading] = useState<boolean>(true);
   const [error, setError] = useState<any>(null);

   useEffect(() => {
      setLoading(true);

      const existingTags = currentTags
         ? currentTags.filter((t) => {
              return t.trim() !== '';
           })
         : // .sort()
           null;

      setExistingTags(existingTags);

      const paramsForApi = {
         count: maxTags, // maxTags from props can be undefined, handled by fetchTags
         tags: existingTags,
      };

      useFetchTags(paramsForApi)
         .then((data) => {
            if (Array.isArray(data)) {
               setNewTags(data);
            } else if (data && Array.isArray(data.tags)) {
               setNewTags(data.tags);
            } else {
               console.warn('Unexpected data structure from fetchTags:', data);
               setNewTags([]);
            }
         })
         .catch((error) => {
            setError(error);
            console.error(error);
         })
         .finally(() => {
            setLoading(false);
         });
   }, [maxTags, currentTags]);

   return (
      <div className='flex flex-col gap-2'>
         <h2 className='text-xl font-bold'>Tag Cloud</h2>
         {isLoading && <p>Loading Tags...</p>}
         {error && <p>Error: {error.message}</p>}
         <TagBadges tags={newTags} existingTags={existingTags} />
      </div>
   );
}
