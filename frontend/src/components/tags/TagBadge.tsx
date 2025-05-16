import React from 'react';
import Link from 'next/link';

import { cn } from '@/lib/utils';

import { badgeVariants } from '../ui/badge';

type Props = {
   name: string;
   className?: string;
   href: string;
   count: number;
   totalCount: number;
};

export function TagBadge({ name, className, href, count, totalCount }: Props) {
   const css = `tag ${className ? className : ''}`;

   const sanitizedHref = `${href.replace(/ /g, '+')}`;

   const tagSize =
      count === totalCount
         ? 1
         : totalCount > 0 && count / totalCount >= 0.0001
           ? 1 + (count / totalCount) * 5
           : 0.8;

   return (
      <span key={name} className={css}>
         <Link
            href={sanitizedHref}
            className={cn(badgeVariants({ variant: 'default' }), `text-[${tagSize}rem]`, css)}
            title={`${name} Tag`}>
            {name}
         </Link>
      </span>
   );
}
