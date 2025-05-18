import React from 'react';

import { TagBadge } from '@/components/tags/TagBadge';
import { LinkTag } from '@/models/LinkTag';
import { cn } from '@/lib/utils';

type TagPillsProps = {
   tags: LinkTag[];
   existingTags?: string[] | null;
};

export function TagBadges({ tags, existingTags }: TagPillsProps) {
   // Total `count` from all tags
   // This is used to determine the size of the tag badges
   let totalSumCount = 0;

   let tagQuery: string = '';

   console.log('TagBadges', tags, existingTags);

   existingTags
      //?.sort((a, b) => (a || '').localeCompare(b || ''))
      ?.forEach((tag) => {
         if (tag.length > 0) tagQuery += `${tag.replace(/ /g, '+')},`;
      });

   tags.forEach((tag) => {
      totalSumCount += tag.count || 0;
   });

   let tagQueryString = tagQuery.length > 0 ? `?tags=${tagQuery}` : '?tags=';

   const results = tags
      ? tags.map((tag: LinkTag, idx: number) => {
           if (tag.name && tag.name.length > 0) {
              //TODO: Add this tag to the collection of existing tags, then sort.
              const tagId = `tag${(idx % 5) + 1}`;

              // So not to append all tags to the query string,
              const adjustedQueryString =
                 tagQueryString + tag.name.replace(/ /g, '+');

              const href = `/links/${adjustedQueryString}`;

              return (
                 <TagBadge
                    key={tag.name}
                    href={href}
                    name={tag.name || ''}
                    count={tag.count || 0}
                    tagId={tagId}
                    totalCount={tag.count || 0}
                 />
              );
           }
        })
      : [];

   return (
      <>
         <div className={cn('flex flex-wrap gap-2 ')}>{results}</div>
      </>
   );
}
