import React from 'react';
import Link from 'next/link';

import { cn } from '@/lib/utils';

import { badgeVariants } from '../ui/badge';

type Props = {
   name: string;
   className?: string;
   href: string;
   count: number;
   tagId: string;
   totalCount: number;
};

export function TagBadge({
   name,
   className,
   href,
   count,
   tagId,
   totalCount,
}: Props) {
   const css = `tag ${className ? className : ''}`;

   const sanitizedHref = `${href.replace(/ /g, '+')}`;

   const tagSize =
      count === totalCount
         ? 1
         : totalCount > 0 && count / totalCount >= 0.0001
           ? 1 + (count / totalCount) * 5
           : 0.8;

   return (
      <Link
         key={name}
         href={sanitizedHref}
         className={cn('tag', tagId)}
         title={`${name} Tag`}>
         {name}
      </Link>
   );
}
