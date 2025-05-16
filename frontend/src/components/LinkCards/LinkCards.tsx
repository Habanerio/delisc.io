import React from 'react';

import LinkCard from './LinkCard';
import { LinkItem } from '@/models/LinkItem';

type LinkCardsProps = {
   items: LinkItem[] | undefined;
};

export default function LinkCards({ items = [] }: LinkCardsProps) {
   return (
      <>
         <div className='links-list grid grid-cols-4 gap-4'>
            {items.map((link: LinkItem, idx: number) => {
               const priority = idx < 6;
               return <LinkCard key={link.id} {...link} />;
            })}
         </div>
      </>
   );
}
