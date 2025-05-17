import React from 'react';

import { TagBadge } from '@/components/tags/TagBadge';
import { LinkTag } from '@/models/LinkTag';
import { cn } from '@/lib/utils';

type TagPillsProps = {
   tags: LinkTag[];
};

export function TagBadges({ tags }: TagPillsProps) {
   let totalCount = 0;

   tags.forEach((tag) => {
      totalCount += tag.count || 0;
   });

   const results = tags
      ? tags.map((tag: LinkTag, idx: number) => {
           const tagId = `tag-${(idx % 10) + 1}`;
           const href = `/tags/${tag.name}`;

           return (
              <TagBadge
                 key={tag.name}
                 href={href}
                 name={tag.name || ''}
                 className={tagId}
                 count={tag.count || 0}
                 totalCount={tag.count || 0}
              />
           );
        })
      : [];

   return (
      <>
         <span className={cn('flex flex-wrap gap-1 justify-start')}>{results}</span>
      </>
   );
}
